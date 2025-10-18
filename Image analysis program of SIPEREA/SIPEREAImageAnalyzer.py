# Last Modified: 2025-06-27c

# === Standard Library Imports ===
import os
import sys
import subprocess
import glob
import csv
import datetime
from datetime import datetime
import shutil
import math
import ctypes
import threading
import queue
import time
from threading import Thread

# === Third-Party Library Imports ===
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
import cv2
from PIL import Image, ImageTk

# === Machine Learning / Deep Learning Imports ===
import tensorflow as tf
from tensorflow.keras.models import load_model, Model  # type: ignore
from tensorflow.keras.layers import Conv2D, MaxPooling2D, UpSampling2D, Input, Concatenate, Dropout  # type: ignore
from tensorflow.keras.optimizers import Adam  # type: ignore
from tensorflow.keras.callbacks import ModelCheckpoint, EarlyStopping, ReduceLROnPlateau, Callback  # type: ignore
from tensorflow.keras.losses import BinaryCrossentropy  # type: ignore
from tensorflow.keras.preprocessing.image import ImageDataGenerator  # type: ignore

from sklearn.metrics import (
    confusion_matrix,
    accuracy_score,
    f1_score,
    precision_score,
    recall_score,
)
from sklearn.model_selection import train_test_split

# === GUI Imports ===
import tkinter as tk
from tkinter import filedialog, ttk

# === Unused/Unusual Imports (should be reviewed for necessity) ===
from ast import IsNot
import pkg_resources
isStop = False

try:
    ctypes.windll.shcore.SetProcessDpiAwareness(1)
except:
    pass


def install_required_packages():
    required_packages = {
        'tensorflow': 'tensorflow',
        'scikit-learn': 'sklearn',
        'opencv-python': 'cv2',
        'numpy': 'numpy',
        'matplotlib': 'matplotlib.pyplot',
        'pillow': 'PIL',
        'tk': 'tkinter'
    }
    
    missing_packages = []
    
    for package, import_name in required_packages.items():
        try:
            __import__(import_name.split('.')[0])
        except ImportError:
            missing_packages.append(package)
    
    if missing_packages:
        print(f"Installing missing packages: {', '.join(missing_packages)}")
        for package in missing_packages:
            subprocess.check_call([sys.executable, "-m", "pip", "install", package])
        print("All required packages have been installed.")
        
# Install required packages if not already installed
install_required_packages()

class SIPEREAImageAnalyzer(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("SIPEREA Image Analyzer")
        self.geometry("1000x600")
        self.minsize(600, 400)
        self.tab_control = ttk.Notebook(self)
        self.tab_control.pack(expand=True, fill=tk.BOTH)
        self.image_analysis_tab = ImageAnalysisTab(self.tab_control)
        self.tab_control.add(self.image_analysis_tab, text="Image Analysis")
        self.training_tab = TrainingTab(self.tab_control)
        self.tab_control.add(self.training_tab, text="Training Segmentation")
        self.protocol("WM_DELETE_WINDOW", self.on_closing)

    def on_closing(self):
        # Close all matplotlib figures
        plt.close('all')
        
        # Cleanup threads if they are running
        if hasattr(self.image_analysis_tab, 'analysis_thread') and self.image_analysis_tab.analysis_thread:
            global isStop
            isStop = True
            self.image_analysis_tab.analysis_thread.join(timeout=1.0)
            
        if hasattr(self.training_tab, 'training_thread') and self.training_tab.training_thread:
            self.training_tab.should_stop = True
            self.training_tab.training_thread.join(timeout=1.0)

        # 프로그램 종료
        self.quit()
        self.destroy()

class ImageAnalysisTab(ttk.Frame):
    def __init__(self, parent):
        super().__init__(parent)

        # Image analysis variables
        self.image = None
        self.displayed_image = None
        self.zoom_factor = 1.0
        self.current_image_index = 0
        self.image_files = []

        # Drag and drop variables
        self.drag_data = {"x": 0, "y": 0, "dragging": False}
        
        # Add message queue for thread communication
        self.message_queue = queue.Queue()
        self.analysis_thread = None

        self.roi_data = None
        self.first_image_time = None

        self.create_widgets()
        self.configure_layout()

        # Create analysis log text widget (initially hidden)
        self.analysis_log = tk.Text(self.canvas_frame, wrap=tk.WORD)
        self.analysis_log_vscroll = ttk.Scrollbar(self.canvas_frame, orient=tk.VERTICAL, command=self.analysis_log.yview)
        self.analysis_log_hscroll = ttk.Scrollbar(self.canvas_frame, orient=tk.HORIZONTAL, command=self.analysis_log.xview)
        self.analysis_log.configure(yscrollcommand=self.analysis_log_vscroll.set,
                                    xscrollcommand=self.analysis_log_hscroll.set)
        
        # Configure grid weights for full expansion
        self.canvas_frame.grid_rowconfigure(0, weight=1)
        self.canvas_frame.grid_columnconfigure(0, weight=1)

    def create_widgets(self):
        # Upper frame for input controls
        self.top_frame = ttk.Frame(self)
        
        # Input folder path entry
        self.input_frame = ttk.Frame(self.top_frame)
        self.path_label = ttk.Label(self.input_frame, text="Input Folder:")
        self.path_entry = ttk.Entry(self.input_frame, width=40)
        self.browse_button = ttk.Button(self.input_frame, text="Browse...", command=self.browse_input_folder)
        
        # Image count label and entry
        self.image_count_label = ttk.Label(self.input_frame, text="Images:")
        self.image_count_entry = ttk.Entry(self.input_frame, width=5, state='readonly')
        
        # Checkboxes for visualization options
        self.export_var = tk.BooleanVar(value=False)
        self.export_checkbox = ttk.Checkbutton(self.input_frame, text="Save binary", variable=self.export_var)

        self.viz_var = tk.BooleanVar(value=False)
        self.enhanced_var = tk.BooleanVar(value=False)
        self.viz_checkbox = ttk.Checkbutton(self.input_frame, text="Save combined", variable=self.viz_var)
        self.enhanced_checkbox = ttk.Checkbutton(self.input_frame, text="Save enhanced", variable=self.enhanced_var)

        # Image viewer frame
        self.viewer_frame = ttk.Frame(self)
        
        # Magnification control buttons and labels
        self.control_frame = ttk.Frame(self.viewer_frame)
        self.zoom_out_button = ttk.Button(self.control_frame, text="-", width=2, command=self.zoom_out)
        self.zoom_label = ttk.Label(self.control_frame, text="100%")
        self.zoom_in_button = ttk.Button(self.control_frame, text="+", width=2, command=self.zoom_in)
        self.analyze_button = ttk.Button(self.control_frame, text="Analyze", command=self.start_analysis)
        self.stop_button = ttk.Button(self.control_frame, text="Stop", command=self.stop_analysis, state=tk.DISABLED)

        # Image navigation buttons
        self.prev_button = ttk.Button(self.control_frame, text="◀", width=2, command=self.show_prev_image)
        self.next_button = ttk.Button(self.control_frame, text="▶", width=2, command=self.show_next_image)
        
        # Add radio buttons for view selection
        self.view_var = tk.StringVar(value="image")  # Default to image view
        self.view_frame = ttk.Frame(self.control_frame)
        ttk.Label(self.view_frame, text="View:").pack(side=tk.LEFT, padx=(20, 5))
        self.image_radio = ttk.Radiobutton(
            self.view_frame, 
            text="Image", 
            value="image", 
            variable=self.view_var,
            command=self.toggle_view
        )
        self.text_radio = ttk.Radiobutton(
            self.view_frame, 
            text="Analysis Log", 
            value="text", 
            variable=self.view_var,
            command=self.toggle_view
        )
        self.image_radio.pack(side=tk.LEFT, padx=(0, 5))
        self.text_radio.pack(side=tk.LEFT)

        # Canvas and scrollbars
        self.canvas_frame = ttk.Frame(self.viewer_frame)
        self.canvas = tk.Canvas(self.canvas_frame, bg="lightgray")
        
        # Scrollbars for the canvas
        self.h_scrollbar = ttk.Scrollbar(self.canvas_frame, orient=tk.HORIZONTAL, command=self.canvas.xview)
        self.v_scrollbar = ttk.Scrollbar(self.canvas_frame, orient=tk.VERTICAL, command=self.canvas.yview)
        self.canvas.configure(xscrollcommand=self.h_scrollbar.set, yscrollcommand=self.v_scrollbar.set)
        
        # Status bar for displaying messages
        self.status_bar = ttk.Label(self, text="Ready", relief=tk.SUNKEN, anchor=tk.W)
        
        # Drag and drop bindings
        self.canvas.bind("<ButtonPress-1>", self.on_drag_start)
        self.canvas.bind("<B1-Motion>", self.on_drag_motion)
        self.canvas.bind("<ButtonRelease-1>", self.on_drag_end)

    def configure_layout(self):
        # Upper frame layout
        self.top_frame.pack(fill=tk.X, padx=10, pady=10)
        
        # Input folder frame layout
        self.input_frame.pack(fill=tk.X, expand=True, pady=(0, 5))
        self.path_label.pack(side=tk.LEFT, padx=(0, 5))
        self.path_entry.pack(side=tk.LEFT, padx=(0, 5), fill=tk.X, expand=True)
        self.browse_button.pack(side=tk.LEFT, padx=(0, 5))
        self.image_count_label.pack(side=tk.LEFT, padx=(5, 5))
        self.image_count_entry.pack(side=tk.LEFT)
        self.export_checkbox.pack(side=tk.LEFT, padx=(5, 5))
        self.viz_checkbox.pack(side=tk.LEFT, padx=(5, 5))
        self.enhanced_checkbox.pack(side=tk.LEFT, padx=(5, 5))
        
        # Image viewer frame layout
        self.viewer_frame.pack(fill=tk.BOTH, expand=True, padx=10, pady=(0, 10))
        
        # Control frame layout
        self.control_frame.pack(fill=tk.X, side=tk.TOP, pady=(0, 5))
        self.prev_button.pack(side=tk.LEFT, padx=(0, 5))
        self.next_button.pack(side=tk.LEFT, padx=(0, 20))
        self.zoom_out_button.pack(side=tk.LEFT)
        self.zoom_label.pack(side=tk.LEFT, padx=5)
        self.zoom_in_button.pack(side=tk.LEFT)
        self.analyze_button.pack(side=tk.LEFT, padx=(20, 5))
        self.stop_button.pack(side=tk.LEFT)
        self.view_frame.pack(side=tk.LEFT, padx=(20, 0))

        # Canvas and scrollbars layout
        self.canvas_frame.pack(fill=tk.BOTH, expand=True)
        self.canvas.grid(row=0, column=0, sticky="nswe")
        self.v_scrollbar.grid(row=0, column=1, sticky="ns") 
        self.h_scrollbar.grid(row=1, column=0, sticky="we")
        
        # Canvas frame grid configuration
        self.canvas_frame.grid_rowconfigure(0, weight=1)
        self.canvas_frame.grid_columnconfigure(0, weight=1)
        
        # Status bar layout
        self.status_bar.pack(fill=tk.X, side=tk.BOTTOM)
        
        # Window resize event binding
        self.bind("<Configure>", self.on_window_resize)

    def browse_input_folder(self):
        folder_path = filedialog.askdirectory()
        if folder_path:
            self.path_entry.delete(0, tk.END)
            self.path_entry.insert(0, folder_path)
            self.scan_image_folder(folder_path)

    def scan_image_folder(self, folder_path):
        # Scan the specified folder for image files
        self.image_files = []
        image_extensions = ('.png', '.jpg', '.jpeg', '.bmp', '.gif', '.tif', '.tiff')
        
        if os.path.isdir(folder_path):
            for file in os.listdir(folder_path):
                if file.lower().endswith(image_extensions):
                    self.image_files.append(os.path.join(folder_path, file))
            
            # Sort image files by name
            self.image_files.sort()
            
            # Display the number of images found
            self.image_count_entry.config(state='normal')
            self.image_count_entry.delete(0, tk.END)
            self.image_count_entry.insert(0, str(len(self.image_files)))
            self.image_count_entry.config(state='readonly')
            
            # If images are found, load the first image
            if self.image_files:
                self.current_image_index = 0
                self.load_image(self.image_files[0])
                
                # Display current image file name in status bar
                current_file = os.path.basename(self.image_files[0])
                self.status_bar.config(text=f"Image 1/{len(self.image_files)}: {current_file}")
            else:
                self.status_bar.config(text="No images found in the selected folder")

    def setup_analysis_mode(self, enable=True):
        if enable:
            # Show text widget if text view is selected
            if self.view_var.get() == "text":
                self.canvas.grid_remove()
                self.analysis_log.grid()
            
            self.analysis_log.delete(1.0, tk.END)  # Clear previous content
            
            # Create thread-safe text redirector
            class ThreadSafeTextRedirector:
                def __init__(self, widget):
                    self.widget = widget
                    self.buffer = ""
                    self.lock = threading.Lock()

                def write(self, str):
                    with self.lock:
                        self.buffer += str
                        if '\n' in self.buffer:
                            lines = self.buffer.split('\n')
                            for line in lines[:-1]:
                                self.widget.after(0, self.write_line, line + '\n')
                            self.buffer = lines[-1]

                def write_line(self, line):
                    self.widget.insert(tk.END, line)
                    self.widget.see(tk.END)

                def flush(self):
                    with self.lock:
                        if self.buffer:
                            self.widget.after(0, self.write_line, self.buffer)
                            self.buffer = ""

            self.old_stdout = sys.stdout
            sys.stdout = ThreadSafeTextRedirector(self.analysis_log)
        else:
            if hasattr(self, 'old_stdout'):
                sys.stdout = self.old_stdout

    def start_analysis(self):
        self.start_analysis_thread()

    def start_analysis_thread(self):
        input_folder = self.path_entry.get()

        if not input_folder or not os.path.isdir(input_folder):
            self.status_bar.config(text="Error: Invalid input folder path")
            return

        output_folder = os.path.join(input_folder, "Output") if self.export_var.get() else None
        viz_folder = os.path.join(input_folder, "Visualization") if (self.viz_var.get() or self.enhanced_var.get()) else None

        if output_folder and not os.path.exists(output_folder):
            os.makedirs(output_folder)
            print(f"Output folder created: {output_folder}")

        if viz_folder and not os.path.exists(viz_folder):
            os.makedirs(viz_folder)
            print(f"Visualization folder created: {viz_folder}")

        global isStop
        isStop = False

        # Configure UI for analysis
        self.status_bar.config(text=f"Analyzing images in {input_folder}...")
        self.analyze_button.config(state=tk.DISABLED)
        self.stop_button.config(state=tk.NORMAL)

        # Setup analysis mode
        self.setup_analysis_mode(True)

        # Create and start analysis thread
        self.analysis_thread = Thread(target=self.run_analysis, args=(input_folder, output_folder, viz_folder), daemon=True)
        self.analysis_thread.start()

        # Start progress checking
        self.check_analysis_progress()

    def run_analysis(self, input_folder, output_folder, viz_folder):
        try:
            # Verify paths
            if not input_folder or not os.path.isdir(input_folder):
                raise ValueError("Invalid input folder path")

            # Load ROI data
            self.read_roi_data(input_folder)
            self.first_image_time = None

            # Prepare result file path
            result_path = os.path.join(input_folder, 'resultArea.csv')
            is_first_result = True  # 첫 번째 이미지인지 여부 플래그

            # Initialize binarizer model
            img_height, img_width = 512, 512
            binarizer = DuckweedBinarizer(img_height=img_height, img_width=img_width)
            binarizer.build_model()

            trained_model_path = 'trained_Model.keras'
            if not os.path.exists(trained_model_path):
                raise ValueError(f"Error: '{trained_model_path}' file not found")

            print(f"'{trained_model_path}' : loading and analyzing")
            binarizer.load_model(trained_model_path)

            # Process each image file
            for idx, img_path in enumerate(sorted(self.image_files), 1):
                if isStop:
                    print("Paused by user. Processing stopped")
                    return

                # Display current image file in status bar
                current_file = os.path.basename(img_path)
                self.status_bar.config(text=f"Image {idx}/{len(self.image_files)}: {current_file}")

                # Process image
                binary_result = ProcessImage(
                    img_path,
                    output_folder,
                    viz_folder,
                    save_viz=self.viz_var.get(),
                    save_enhanced=self.enhanced_var.get(),
                    binarizer=binarizer
                )

                if binary_result is not None:
                    results = self.process_images_with_roi(binary_result, img_path)
                    if results:
                        df = pd.DataFrame([results])
                        write_mode = 'w' if is_first_result else 'a'
                        header = is_first_result
                        df.to_csv(result_path, mode=write_mode, index=False, header=header)
                        is_first_result = False

                plt.close('all')

            self.message_queue.put(("completed", None))
        except Exception as e:
            self.message_queue.put(("error", str(e)))
            plt.close('all')

    def check_analysis_progress(self):
        try:
            message_type, message_data = self.message_queue.get_nowait()
            
            if message_type == "completed":
                if not isStop:
                    self.status_bar.config(text="Analysis completed")
                else:
                    self.status_bar.config(text="Analysis stopped")
                self.cleanup_after_analysis()
            elif message_type == "error":
                self.status_bar.config(text=f"Analysis failed: {message_data}")
                self.cleanup_after_analysis()
            
        except queue.Empty:
            # No messages yet, check again in 100ms
            if self.analysis_thread.is_alive():
                self.after(100, self.check_analysis_progress)
            else:
                self.cleanup_after_analysis()

    def cleanup_after_analysis(self):
        self.analyze_button.config(state=tk.NORMAL)
        self.stop_button.config(state=tk.DISABLED)
        if hasattr(self, 'old_stdout'):
            sys.stdout = self.old_stdout

    def stop_analysis(self):
        global isStop
        isStop = True
        self.status_bar.config(text="Stopping analysis...")
        
        Thread(target=self.wait_for_analysis_thread, daemon=True).start()

    def wait_for_analysis_thread(self):
        if self.analysis_thread and self.analysis_thread.is_alive():
            self.analysis_thread.join()
            self.after(100, self.cleanup_after_analysis)

    def load_image(self, image_path):
        try:
            self.current_image_file = image_path
            self.image = Image.open(image_path)
            self.update_display()
            
            # Current image info in status bar
            file_name = os.path.basename(image_path)
            self.status_bar.config(text=f"Image {self.current_image_index + 1}/{len(self.image_files)}: {file_name}")
        except Exception as e:
            self.status_bar.config(text=f"Error loading image: {str(e)}")

    def update_display(self):
        if self.image:
            # Adjust zoom factor and resize image
            width = int(self.image.width * self.zoom_factor)
            height = int(self.image.height * self.zoom_factor)
            resized_image = self.image.resize((width, height), Image.LANCZOS)
            
            # Update the canvas with the resized image
            self.displayed_image = ImageTk.PhotoImage(resized_image)
            self.canvas.delete("all")
            self.canvas.create_image(0, 0, anchor=tk.NW, image=self.displayed_image, tags="image")
            self.canvas.config(scrollregion=(0, 0, width, height))
            
            # Update the scrollbars
            self.zoom_label.config(text=f"{int(self.zoom_factor * 100)}%")

    def zoom_in(self):
        self.zoom_factor *= 1.2
        self.update_display()

    def zoom_out(self):
        self.zoom_factor /= 1.2
        if self.zoom_factor < 0.1:
            self.zoom_factor = 0.1
        self.update_display()

    def show_prev_image(self):
        if not self.image_files:
            return
            
        self.current_image_index = (self.current_image_index - 1) % len(self.image_files)
        self.load_image(self.image_files[self.current_image_index])
        
        # Display current image information in status bar
        current_file = os.path.basename(self.image_files[self.current_image_index])
        self.status_bar.config(text=f"Image {self.current_image_index + 1}/{len(self.image_files)}: {current_file}")

    def show_next_image(self):
        if not self.image_files:
            return
            
        self.current_image_index = (self.current_image_index + 1) % len(self.image_files)
        self.load_image(self.image_files[self.current_image_index])
        
        # Display current image information in status bar
        current_file = os.path.basename(self.image_files[self.current_image_index])
        self.status_bar.config(text=f"Image {self.current_image_index + 1}/{len(self.image_files)}: {current_file}")

    def toggle_view(self):
        if self.view_var.get() == "image":
            # Show image canvas with its scrollbars
            self.canvas.grid(row=0, column=0, sticky="nsew")
            self.v_scrollbar.grid(row=0, column=1, sticky="ns")
            self.h_scrollbar.grid(row=1, column=0, sticky="we")
            
            # Hide analysis log and its scrollbars
            self.analysis_log.grid_remove()
            self.analysis_log_vscroll.grid_remove()
            self.analysis_log_hscroll.grid_remove()
            
            if hasattr(self, 'current_image_file'):
                self.load_image(self.current_image_file)
        else:
            self.canvas.grid_remove()
            self.v_scrollbar.grid_remove()
            self.h_scrollbar.grid_remove()
            
            self.analysis_log.grid(row=0, column=0, sticky="nsew")
            self.analysis_log_vscroll.grid(row=0, column=1, sticky="ns")
            self.analysis_log_hscroll.grid(row=1, column=0, sticky="we")

    def on_drag_start(self, event):
        # Save the drag start position
        self.drag_data["x"] = event.x
        self.drag_data["y"] = event.y
        self.drag_data["dragging"] = True
        self.canvas.config(cursor="fleur")

    def on_drag_motion(self, event):
        # Update the drag position if dragging
        if self.drag_data["dragging"]:
            # Compute the delta movement
            delta_x = event.x - self.drag_data["x"]
            delta_y = event.y - self.drag_data["y"]
            
            scroll_region = self.canvas.cget("scrollregion").split()
            if len(scroll_region) == 4:
                x_min, y_min, x_max, y_max = map(int, scroll_region)
                canvas_width = x_max - x_min
                canvas_height = y_max - y_min
                
                # Calculate the visible area
                visible_width = self.canvas.winfo_width()
                visible_height = self.canvas.winfo_height()
                
                # Compute the scroll amount
                x_move = -delta_x / visible_width
                y_move = -delta_y / visible_height
                
                # Move the canvas view
                self.canvas.xview_moveto(self.canvas.xview()[0] + x_move)
                self.canvas.yview_moveto(self.canvas.yview()[0] + y_move)
            
            # Update the drag start position
            self.drag_data["x"] = event.x
            self.drag_data["y"] = event.y

    def on_drag_end(self, event):
        # Stop dragging
        self.drag_data["dragging"] = False
        self.canvas.config(cursor="")

    def on_window_resize(self, event):
        # Window resize event handler
        if event.widget == self:
            # Avoid multiple calls during resize
            self.after_cancel(self.resize_job) if hasattr(self, 'resize_job') else None
            self.resize_job = self.after(100, self.update_display)

    def read_roi_data(self, folder_path):
        roi_path = os.path.join(folder_path, 'ROI.csv')
        if os.path.exists(roi_path):
            self.roi_data = pd.read_csv(roi_path)
            print(f"ROI loading completed: ROI count = {len(self.roi_data)}")
            return True
        else:
            print("ROI.csv not found. Single ROI assigned")
            return False

    def count_white_pixels(self, binary_image, roi_info):
        isDeubgging = False
        x1, y1, x2, y2 = int(roi_info['X1']), int(roi_info['Y1']), int(roi_info['X2']), int(roi_info['Y2'])
    
        # Compute the extended ROI region
        img_height, img_width = binary_image.shape
        extended_x2 = min(x2 + 1, img_width)
        extended_y2 = min(y2 + 1, img_height)
    
        roi_region = binary_image[y1:extended_y2, x1:extended_x2]
    
        if isDeubgging:
            roi_filename = f"roi_x{x1}y{y1}_{extended_x2}y{extended_y2}.png"
            cv2.imwrite(roi_filename, roi_region)
    
        if roi_info['Shape'].lower() == 'circle':
            # Compute the center and radius for the circular ROI
            height, width = roi_region.shape
            center_x = width // 2
            center_y = height // 2
            radius = min(width, height) // 2 + 1
        
            # Generate a grid of coordinates
            y, x = np.ogrid[:height, :width]
        
            # Compute the distance from the center
            dist_from_center = (x - center_x)**2 + (y - center_y)**2
        
            # Generate a mask for the circular ROI
            mask = dist_from_center <= radius**2
        
            # Generate the mask image
            mask_image = mask.astype(np.uint8) * 255            
                
            if isDeubgging:
                mask_filename = f"mask_x{x1}y{y1}_{extended_x2}y{extended_y2}.png"
                cv2.imwrite(mask_filename, mask_image)

            # Count white pixels in the circular ROI
            white_pixels = (roi_region == 255) & mask
            roi_region_masked = white_pixels.astype(np.uint8) * 255
            if isDeubgging:
                result_filename = f"result_x{x1}y{y1}_{extended_x2}y{extended_y2}.png"
                cv2.imwrite(result_filename, roi_region_masked)

            white_count = np.sum(white_pixels)
            print(f"ROI at ({x1}, {y1}, {extended_x2}, {extended_y2}): {white_count} white pixels")
    
            return white_count
        else:
            # Compute the rectangular ROI region
            white_count = np.sum(roi_region == 255)
            print(f"ROI at ({x1}, {y1}, {extended_x2}, {extended_y2}): {white_count} white pixels")
            return white_count

    def create_column_name(self, roi_row):
        parts = []
        if not pd.isna(roi_row['Condition1']):
            parts.append(str(roi_row['Condition1']))
        if not pd.isna(roi_row['Condition2']):
            parts.append(str(roi_row['Condition2']))
        if not pd.isna(roi_row['Plate#']):
            parts.append(f"P{roi_row['Plate#']}")
        return "||".join(parts)

    def process_images_with_roi(self, binary_image, image_file):
        results = {}
        
        filename = os.path.basename(image_file)
        
        try:
            parts = filename.split(' ')
            if len(parts) < 2:
                print(f"Warning: Invalid filename format for {filename}. Expected format: 'YYYY-MM-DD (HH-mm-ss-mmm)'")
                return None

            # Parse the date part
            date_str = parts[0]
            if not date_str.count('-') == 2:
                print(f"Warning: Invalid date format in {filename}. Expected 'YYYY-MM-DD'")
                return None
            year, month, day = map(int, date_str.split('-'))

            # Parse the time part
            time_part = parts[1].split('-(')[0].strip('()')
            if not time_part.count('-') == 3:
                print(f"Warning: Invalid time format in {filename}. Expected '(HH-mm-ss-mmm)'")
                return None
            hour, minute, second, millisec = map(int, time_part.split('-'))
            
            # Create datetime object
            current_time = datetime(year, month, day, hour, minute, second, millisec * 1000)
            
            # Get the first image time if not set
            if self.first_image_time is None:
                self.first_image_time = current_time
            
            # Compute the time difference in days
            time_diff = (current_time - self.first_image_time).total_seconds() / (24 * 3600)
            
            # Save the results
            results['File name'] = filename
            results['Time(day)'] = f"{time_diff:.3f}"
            
            # Process each ROI
            if self.roi_data is None:
                height, width = binary_image.shape
                dummy_roi = {
                    'Shape': 'rectangle',
                    'X1': 0, 'Y1': 0,
                    'X2': width, 'Y2': height,
                    'Condition1': 'Total'
                }
                white_pixels = self.count_white_pixels(binary_image, dummy_roi)
                results['Total'] = white_pixels
            else:
                for _, roi in self.roi_data.iterrows():
                    column_name = self.create_column_name(roi)
                    white_pixels = self.count_white_pixels(binary_image, roi)
                    results[column_name] = white_pixels
            
            return results
            
        except (ValueError, IndexError) as e:
            print(f"Warning: Could not process {filename}. Error: {str(e)}")
            return None

    def save_results(self, all_results, input_path):

        df = pd.DataFrame(all_results)
        result_path = os.path.join(input_path, 'resultArea.csv')
        df.to_csv(result_path, index=False)
        print(f"Finale result file saved: {result_path}")


class TrainingTab(ttk.Frame):
    def __init__(self, parent):
        super().__init__(parent)
        
        # Add message queue for thread communication
        self.message_queue = queue.Queue()
        self.training_thread = None
        self.should_stop = False
        
        self.original_stdout = sys.stdout
        self.create_widgets()
        self.configure_layout()

    def create_widgets(self):
        self.folder_frames = ttk.LabelFrame(self, text="Folders")
        
        self.source_frame = self.create_folder_input_frame(
            self.folder_frames, "Source Image Folder:", self.browse_source_folder)
        
        self.binary_frame = self.create_folder_input_frame(
            self.folder_frames, "Binary Image Folder:", self.browse_binary_folder)
        
        self.params_frame = ttk.LabelFrame(self, text="Training Parameters")
        
        # Epochs
        self.epochs_frame = ttk.Frame(self.params_frame)
        ttk.Label(self.epochs_frame, text="Epochs:").pack(side=tk.LEFT, padx=(0, 5))
        self.epochs_var = tk.StringVar(value="150")
        self.epochs_entry = ttk.Entry(self.epochs_frame, width=10, textvariable=self.epochs_var)
        self.epochs_entry.pack(side=tk.LEFT)

        # Early Stopping Patience
        self.es_patience_frame = ttk.Frame(self.params_frame)
        ttk.Label(self.es_patience_frame, text="Early Stopping Patience:").pack(side=tk.LEFT, padx=(0, 5))
        self.es_patience_var = tk.StringVar(value="30")
        self.es_patience_entry = ttk.Entry(self.es_patience_frame, width=10, textvariable=self.es_patience_var)
        self.es_patience_entry.pack(side=tk.LEFT)

        # Reduce LR Patience
        self.lr_patience_frame = ttk.Frame(self.params_frame)
        ttk.Label(self.lr_patience_frame, text="Reduce LR Patience:").pack(side=tk.LEFT, padx=(0, 5))
        self.lr_patience_var = tk.StringVar(value="10")
        self.lr_patience_entry = ttk.Entry(self.lr_patience_frame, width=10, textvariable=self.lr_patience_var)
        self.lr_patience_entry.pack(side=tk.LEFT)

        # Save combined and Save enhanced checkboxes
        self.viz_options_frame = ttk.Frame(self.params_frame)
        self.save_combined_var = tk.BooleanVar(value=True)
        self.save_enhanced_var = tk.BooleanVar(value=True)
        
        self.save_combined_cb = ttk.Checkbutton(
            self.viz_options_frame, 
            text="Save combined",
            variable=self.save_combined_var
        )
        
        self.save_enhanced_cb = ttk.Checkbutton(
            self.viz_options_frame, 
            text="Save enhanced",
            variable=self.save_enhanced_var
        )
        
        # Button frame
        self.button_frame = ttk.Frame(self)
        self.start_button = ttk.Button(self.button_frame, text="Start Training", 
                                       command=self.start_training)
        self.stop_button = ttk.Button(self.button_frame, text="Stop Training", 
                                      command=self.stop_training, state=tk.DISABLED)
        
        self.log_frame = ttk.LabelFrame(self, text="Training Log")
        self.log_text = tk.Text(self.log_frame, wrap=tk.WORD, height=15)
        self.log_scroll = ttk.Scrollbar(self.log_frame, orient=tk.VERTICAL, 
                                        command=self.log_text.yview)
        self.log_text.configure(yscrollcommand=self.log_scroll.set)
        
        # Status bar
        self.status_bar = ttk.Label(self, text="Ready", relief=tk.SUNKEN, anchor=tk.W)

    def configure_layout(self):
        # Folder frames layout
        self.folder_frames.pack(fill=tk.X, padx=10, pady=10)
        self.source_frame.pack(fill=tk.X, pady=(0, 5))
        self.binary_frame.pack(fill=tk.X, pady=(0, 5))
        
        # Training Parameters Frame layout
        self.params_frame.pack(fill=tk.X, padx=10, pady=10)
        self.epochs_frame.pack(side=tk.LEFT, padx=(10, 5))
        self.es_patience_frame.pack(side=tk.LEFT, padx=(10, 5))
        self.lr_patience_frame.pack(side=tk.LEFT, padx=(10, 5))
        self.viz_options_frame.pack(side=tk.LEFT, padx=(10, 5))
        self.save_combined_cb.pack(side=tk.LEFT, padx=(5, 5))
        self.save_enhanced_cb.pack(side=tk.LEFT, padx=(5, 5))
        
        # Button frame layout
        self.button_frame.pack(fill=tk.X, padx=10, pady=5)
        self.start_button.pack(side=tk.LEFT, padx=(5, 5))
        self.stop_button.pack(side=tk.LEFT)
        
        # Log frame layout
        self.log_frame.pack(fill=tk.BOTH, expand=True, padx=10, pady=10)
        self.log_text.grid(row=0, column=0, sticky="nsew")
        self.log_scroll.grid(row=0, column=1, sticky="ns")
        self.log_frame.grid_rowconfigure(0, weight=1)
        self.log_frame.grid_columnconfigure(0, weight=1)
        
        # Status bar layout
        self.status_bar.pack(fill=tk.X, side=tk.BOTTOM, padx=10, pady=(0, 10))

    def create_folder_input_frame(self, parent, label_text, browse_command):
        frame = ttk.Frame(parent)
    
        label = ttk.Label(frame, text=label_text)
        label.pack(side=tk.LEFT, padx=(0, 5))
    
        entry = ttk.Entry(frame, width=40, state='normal')  # 초기 상태를 'disabled'로 설정
        entry.pack(side=tk.LEFT, padx=(0, 5), fill=tk.X, expand=True)
    
        browse_button = ttk.Button(frame, text="Browse...", command=browse_command, state='normal')  # 초기 상태를 'disabled'로 설정
        browse_button.pack(side=tk.LEFT)
    
        # Store entry widget as attribute of frame for easy access
        frame.entry = entry
        frame.button = browse_button
    
        return frame

    def browse_source_folder(self):
        folder_path = filedialog.askdirectory()
        if folder_path:
            self.source_frame.entry.delete(0, tk.END)
            self.source_frame.entry.insert(0, folder_path)

    def browse_binary_folder(self):
        folder_path = filedialog.askdirectory()
        if folder_path:
            self.binary_frame.entry.delete(0, tk.END)
            self.binary_frame.entry.insert(0, folder_path)

    def start_training(self):
        # Validate paths
        source_dir = self.source_frame.entry.get()
        binary_dir = self.binary_frame.entry.get()

        if not source_dir or not binary_dir:
            self.status_bar.config(text="Error: Source and Binary folder paths must be specified")
            return

        if not os.path.isdir(source_dir) or not os.path.isdir(binary_dir):
            self.status_bar.config(text="Error: One or more specified folder paths are invalid")
            return

        # Automatically create subfolders in the source folder
        output_folder = os.path.join(source_dir, "Output")
        viz_folder = os.path.join(source_dir, "Visualization") if (self.save_combined_var.get() or self.save_enhanced_var.get()) else None

        if not os.path.exists(output_folder):
            os.makedirs(output_folder)
            print(f"Output folder created: {output_folder}")

        if viz_folder and not os.path.exists(viz_folder):
            os.makedirs(viz_folder)
            print(f"Visualization folder created: {viz_folder}")

        # Configure UI for training
        self.should_stop = False
        self.status_bar.config(text="Training in progress...")
        self.start_button.config(state=tk.DISABLED)
        self.stop_button.config(state=tk.NORMAL)
        
        # Clear log
        self.log_text.delete(1.0, tk.END)
        
        # Record training start time
        self.training_start_time = time.time()


        self.training_thread = Thread(target=self.run_training_thread, args=(source_dir, binary_dir, output_folder, viz_folder), daemon=True)
        self.training_thread.start()
        
        self.check_training_progress()

    def run_training_thread(self, source_dir, binary_dir, output_folder, viz_folder):
        try:
            sys.stdout = self.create_log_redirector()
        
            # Get training parameters from UI
            try:
                epochs = int(self.epochs_var.get())
                es_patience = int(self.es_patience_var.get())
                lr_patience = int(self.lr_patience_var.get())
            except ValueError:
                raise ValueError("Training parameters must be valid integers")
            
            img_height, img_width = 512, 512
            binarizer = DuckweedBinarizer(img_height=img_height, img_width=img_width)
            binarizer.build_model()

            image_paths, mask_paths = collect_dataset(source_dir, binary_dir)
            if not image_paths:
                raise ValueError("No training images found")
            
            print(f"Total {len(image_paths)} images are used for training")
            check_mask_quality(mask_paths)
        
            print("Initiating training...")
            history = binarizer.train(
                image_paths=image_paths,
                mask_paths=mask_paths,
                epochs=epochs,
                batch_size=2,
                should_stop=lambda: self.should_stop,
                es_patience=es_patience,
                lr_patience=lr_patience
            )
        

            # Record training end time and calculate elapsed time
            training_end_time = time.time()
            elapsed_time = training_end_time - self.training_start_time
            print(f"Training completed in {elapsed_time:.2f} s")


            # Early stopping or normal completion
            if history is not None and not self.should_stop:
                print("Training completed. Starting prediction...")
            
                # Save trained model
                binarizer.save_model('bestTrained_Model.keras')
                print("'bestTrained_Model.keras' saved")
            

                os.makedirs(output_folder, exist_ok=True)

                for img_path in image_paths:
                    if self.should_stop:
                        print("Process stopped by user during prediction.")
                        break

                    filename = os.path.basename(img_path)
                    output_path = os.path.join(output_folder, filename)
                    print(f"Predicting and saving binary mask for: {filename}")

                    # Predict binary mask
                    binary_prediction = binarizer.predict(
                        img_path,
                        save_path=viz_folder,
                        save_viz=self.save_combined_var.get(),
                        save_enhanced=self.save_enhanced_var.get()
                    )

                    # Save binary mask to output folder
                    cv2.imwrite(output_path, binary_prediction)

                print(f"Binary images generated and saved to: {output_folder}")



                # Run predictions on validation data for ROC-AUC analysis
                print("Performing ROC-AUC analysis...")
                X, y = binarizer.prepare_training_data(image_paths, mask_paths)
                y_pred = binarizer.model.predict(X, verbose=0).flatten()
                y_true = y.flatten()

                # Compute ROC-AUC
                from sklearn.metrics import roc_curve, auc
                fpr, tpr, thresholds = roc_curve(y_true, y_pred)
                roc_auc = auc(fpr, tpr)
                print(f"ROC-AUC: {roc_auc:.4f}")

                # Save ROC-AUC raw data to CSV
                roc_data = pd.DataFrame({
                    'False positive rate': fpr,
                    'True positive rate': tpr,
                    'Thresholds': thresholds
                })
                #roc_data_path = os.path.join(output_folder, "roc_auc_data.csv")
                #roc_data.to_csv(roc_data_path, index=False)
                #print(f"ROC-AUC data saved to {roc_data_path}")

                # Save ROC-AUC raw data to CSV (with sampling)
                sampled_indices = np.linspace(0, len(fpr) - 1, num=1000, dtype=int)
                fpr_sampled = fpr[sampled_indices]
                tpr_sampled = tpr[sampled_indices]
                thresholds_sampled = thresholds[sampled_indices]

                roc_data_sampled = pd.DataFrame({
                    'False positive rate': fpr_sampled,
                    'True positive rate': tpr_sampled,
                    'Thresholds': thresholds_sampled
                })
                roc_data_path = os.path.join(output_folder, "roc_auc_data_sampled.csv")
                roc_data_sampled.to_csv(roc_data_path, index=False)
                print(f"Sampled ROC-AUC data saved to {roc_data_path}")

                # Save ROC-AUC plot to PNG
                plt.figure()
                plt.plot(fpr, tpr, color='darkorange', lw=2, label=f'ROC curve (area = {roc_auc:.4f})')
                plt.plot([0, 1], [0, 1], color='navy', lw=2, linestyle='--')
                plt.xlabel('False positive rate')
                plt.ylabel('True positive rate')
                plt.title('Receiver Operating Characteristic (ROC)')
                plt.legend(loc="lower right")
                roc_plot_path = os.path.join(output_folder, "roc_auc_plot.png")
                plt.savefig(roc_plot_path)
                plt.close()
                print(f"ROC-AUC plot saved to {roc_plot_path}")
                   

                # Compute segmentation metrics
                # 1. Binarize predictions and ground truth
                y_pred_bin = (y_pred > 0.5).astype(np.uint8)
                y_true_bin = y_true.astype(np.uint8)

                # 2. Pixel Accuracy
                pixel_accuracy = accuracy_score(y_true_bin, y_pred_bin)

                # 3. IoU (Jaccard)
                intersection = np.logical_and(y_true_bin, y_pred_bin).sum()
                union = np.logical_or(y_true_bin, y_pred_bin).sum()
                iou = intersection / union if union > 0 else 0.0

                # 4. Dice Coefficient
                dice = (2 * intersection) / (y_true_bin.sum() + y_pred_bin.sum()) if (y_true_bin.sum() + y_pred_bin.sum()) > 0 else 0.0

                # 5. Precision, Recall
                precision = precision_score(y_true_bin, y_pred_bin, zero_division=0)
                recall = recall_score(y_true_bin, y_pred_bin, zero_division=0)

                # 6. Save segmentation metrics to console
                print(f"Pixel Accuracy: {pixel_accuracy:.4f}")
                print(f"IoU (Jaccard): {iou:.4f}")
                print(f"Dice Coefficient: {dice:.4f}")
                print(f"Precision: {precision:.4f}")
                print(f"Recall: {recall:.4f}")

                # 7. Save segmentation metrics to CSV
                metrics_df = pd.DataFrame([{
                    "Pixel Accuracy": pixel_accuracy,
                    "IoU": iou,
                    "Dice": dice,
                    "Precision": precision,
                    "Recall": recall,
                    "ROC-AUC": roc_auc
                }])
                metrics_path = os.path.join(output_folder, "segmentation_metrics.csv")
                metrics_df.to_csv(metrics_path, index=False)
                print(f"Segmentation metrics saved to {metrics_path}")

                # Compute confusion matrix
                cm = confusion_matrix(y_true_bin.flatten(), y_pred_bin.flatten())
                print("Confusion Matrix:")
                print(cm)

                # Convert confusion matrix to DataFrame for better visualization
                cm_df = pd.DataFrame(
                    cm,
                    index=["True Negative", "True Positive"],
                    columns=["Predicted Negative", "Predicted Positive"]
                )

                # Save confusion matrix to CSV
                cm_path = os.path.join(output_folder, "confusion_matrix.csv")
                cm_df.to_csv(cm_path)
                print(f"Confusion matrix saved to {cm_path}")
                
                print("\n[INFO] Training analysis completed!")
                               
 
            # Close any remaining figures
            plt.close('all')
    
            # Send completion message
            self.message_queue.put(("completed", None))

        except Exception as e:
            # Send error message
            self.message_queue.put(("error", str(e)))
        finally:
            # Restore stdout
            sys.stdout = self.original_stdout
            plt.close('all')
            print("\n[INFO] Training analysis completed!")

    # Check training progress and update UI
    def check_training_progress(self):
        try:
            msg_type, msg_data = self.message_queue.get_nowait()
            
            if msg_type == "completed":
                if not self.should_stop:
                    self.status_bar.config(text="Training completed")
                else:
                    self.status_bar.config(text="Training stopped")
                self.cleanup_after_training()
            elif msg_type == "error":
                self.status_bar.config(text=f"Training failed: {msg_data}")
                self.cleanup_after_training()
            
        except queue.Empty:
            # No messages yet, check again in 100ms
            if self.training_thread and self.training_thread.is_alive():
                self.after(100, self.check_training_progress)
            else:
                self.cleanup_after_training()

    def stop_training(self):
        self.should_stop = True
        self.status_bar.config(text="Stopping training..")
        
        # Wait for training thread to complete in a separate thread
        Thread(target=self.wait_for_training_thread, daemon=True).start()

    def wait_for_training_thread(self):
        if self.training_thread and self.training_thread.is_alive():
            self.training_thread.join()
            # Update UI in main thread
            self.after(100, self.cleanup_after_training)

    def cleanup_after_training(self):
        self.start_button.config(state=tk.NORMAL)
        self.stop_button.config(state=tk.DISABLED)
        if hasattr(self, 'old_stdout'):
            sys.stdout = self.original_stdout

    def create_log_redirector(self):
        class ThreadSafeLogRedirector:
            def __init__(self, widget):
                self.widget = widget
                self.buffer = ""
                self.lock = threading.Lock()

            def write(self, str):
                with self.lock:
                    self.buffer += str
                    if '\n' in self.buffer:
                        lines = self.buffer.split('\n')
                        for line in lines[:-1]:
                            self.widget.after(0, self.write_line, line + '\n')
                        self.buffer = lines[-1]

            def write_line(self, line):
                self.widget.insert(tk.END, line)
                self.widget.see(tk.END)

            def flush(self):
                with self.lock:
                    if self.buffer:
                        self.widget.after(0, self.write_line, self.buffer)
                        self.buffer = ""

        return ThreadSafeLogRedirector(self.log_text)

class DuckweedBinarizer:
    def __init__(self, img_height=256, img_width=256, resize_for_model=True):
        self.img_height = img_height
        self.img_width = img_width
        self.resize_for_model = resize_for_model
        self.model = None

    def build_model(self):
        tf.keras.backend.clear_session()
        
        inputs = Input(shape=(self.img_height, self.img_width, 3))

        # Encoder
        conv1 = Conv2D(32, (3, 3), activation='relu', padding='same')(inputs)
        conv1 = Conv2D(32, (3, 3), activation='relu', padding='same')(conv1)
        pool1 = MaxPooling2D((2, 2))(conv1)

        conv2 = Conv2D(64, (3, 3), activation='relu', padding='same')(pool1)
        conv2 = Conv2D(64, (3, 3), activation='relu', padding='same')(conv2)
        pool2 = MaxPooling2D((2, 2))(conv2)

        conv3 = Conv2D(128, (3, 3), activation='relu', padding='same')(pool2)
        conv3 = Conv2D(128, (3, 3), activation='relu', padding='same')(conv3)
        pool3 = MaxPooling2D((2, 2))(conv3)

        # Bottleneck
        conv4 = Conv2D(256, (3, 3), activation='relu', padding='same')(pool3)
        conv4 = Dropout(0.5)(conv4)
        conv4 = Conv2D(256, (3, 3), activation='relu', padding='same')(conv4)

        # Decoder
        up3 = UpSampling2D((2, 2))(conv4)
        up3 = Conv2D(128, (2, 2), activation='relu', padding='same')(up3)
        merge3 = Concatenate()([conv3, up3])
        conv5 = Conv2D(128, (3, 3), activation='relu', padding='same')(merge3)
        conv5 = Conv2D(128, (3, 3), activation='relu', padding='same')(conv5)

        up2 = UpSampling2D((2, 2))(conv5)
        up2 = Conv2D(64, (2, 2), activation='relu', padding='same')(up2)
        merge2 = Concatenate()([conv2, up2])
        conv6 = Conv2D(64, (3, 3), activation='relu', padding='same')(merge2)
        conv6 = Conv2D(64, (3, 3), activation='relu', padding='same')(conv6)

        up1 = UpSampling2D((2, 2))(conv6)
        up1 = Conv2D(32, (2, 2), activation='relu', padding='same')(up1)
        merge1 = Concatenate()([conv1, up1])
        conv7 = Conv2D(32, (3, 3), activation='relu', padding='same')(merge1)
        conv7 = Conv2D(32, (3, 3), activation='relu', padding='same')(conv7)

        conv8 = Conv2D(16, (3, 3), activation='relu', padding='same')(conv7)
        outputs = Conv2D(1, (1, 1), activation='sigmoid')(conv8)

        model = Model(inputs=inputs, outputs=outputs)
        model.compile(optimizer=Adam(learning_rate=0.0001),
                      loss=BinaryCrossentropy(from_logits=False),
                      metrics=['accuracy'])
        
        # Report model summary
        model.summary()
        
        self.model = model
        return model

    def preprocess_image(self, image_path):
        img = cv2.imread(image_path)
        if img is None:
            raise ValueError(f"Could not load image from {image_path}")

        img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        original_height, original_width = img.shape[:2]

        if self.resize_for_model:
            img_resized = cv2.resize(img, (self.img_width, self.img_height), interpolation=cv2.INTER_AREA)
        else:
            img_resized = img

        img_normalized = img_resized / 255.0
        return img_normalized, (original_height, original_width)

    def prepare_training_data(self, image_paths, mask_paths):
        X, y = [], []
        for img_path, mask_path in zip(image_paths, mask_paths):
            img, _ = self.preprocess_image(img_path)
            X.append(img)

            mask = cv2.imread(mask_path, cv2.IMREAD_GRAYSCALE)
            if mask is None:
                raise ValueError(f"Could not load mask from {mask_path}")

            if self.resize_for_model:
                mask = cv2.resize(mask, (self.img_width, self.img_height), interpolation=cv2.INTER_NEAREST)

            mask = (mask > 127).astype(np.float32)
            mask = np.expand_dims(mask, axis=-1)
            y.append(mask)

        return np.array(X), np.array(y)

    def train(self, image_paths, mask_paths, validation_split=0.2, epochs=100, batch_size=8, should_stop=lambda: False, es_patience=30, lr_patience=10):
        if self.model is None:
            self.build_model()

        with tf.device('/CPU:0'):
            X, y = self.prepare_training_data(image_paths, mask_paths)

            from tensorflow.keras.preprocessing.image import ImageDataGenerator

            seed = 42
            data_gen_args = dict(
                rotation_range=15,
                width_shift_range=0.1,
                height_shift_range=0.1,
                shear_range=0.05,
                zoom_range=[0.9, 1.1],
                horizontal_flip=True,
                fill_mode='nearest')

            image_datagen = ImageDataGenerator(**data_gen_args)
            mask_datagen = ImageDataGenerator(**data_gen_args)

            X_train, X_val, y_train, y_val = train_test_split(X, y, test_size=validation_split, random_state=seed)
            print(f"Spliting data sets: Training Count = {X_train.shape[0]}, Validation Count = {X_val.shape[0]}")

            image_generator = image_datagen.flow(X_train, batch_size=batch_size, seed=seed)
            mask_generator = mask_datagen.flow(y_train, batch_size=batch_size, seed=seed)

            def train_generator():
                while True:
                    if should_stop(): 
                        raise StopIteration("Training stopped by user")
                    X_batch = next(image_generator)
                    y_batch = next(mask_generator)
                    yield X_batch, y_batch

            train_steps = (len(X_train) + batch_size - 1) // batch_size

            callbacks = [
                EarlyStopping(monitor='val_loss', patience=es_patience, restore_best_weights=True, verbose=1),
                ModelCheckpoint('current_Best_Model.keras', monitor='val_loss', save_best_only=True, verbose=1),
                ReduceLROnPlateau(monitor='val_loss', factor=0.5, patience=lr_patience, min_lr=1e-6, verbose=1),
                StopTrainingCallback(should_stop) 
            ]

            try:
                history = self.model.fit(
                    train_generator(),
                    steps_per_epoch=train_steps,
                    validation_data=(X_val, y_val),
                    epochs=epochs,
                    callbacks=callbacks,
                    verbose=1
                )
            except StopIteration:
                print("\nTraining paused by user")
                return None

            self._plot_training_history(history)
            
            # Save current_Best_Model.keras after training
            self.model.save('current_Best_Model.keras')
            # Load the best model after training
            self.model = load_model('current_Best_Model.keras')
            
            return history

    def _plot_training_history(self, history):
        # Save training history to CSV
        history_df = pd.DataFrame({
            'epoch': range(1, len(history.history['loss']) + 1),
            'training_loss': history.history['loss'],
            'validation_loss': history.history['val_loss'],
            'training_accuracy': history.history['accuracy'],
            'validation_accuracy': history.history['val_accuracy']
        })
        history_df.to_csv('training_history.csv', index=False)
        print("Training history saved to 'training_history.csv'")

        # Create visualization plot
        plt.figure(figsize=(12, 5))
        plt.subplot(1, 2, 1)
        plt.plot(history.history['loss'], label='Training Loss')
        plt.plot(history.history['val_loss'], label='Validation Loss')
        plt.title('Loss Over Epochs')
        plt.xlabel('Epoch')
        plt.ylabel('Loss')
        plt.legend()

        plt.subplot(1, 2, 2)
        plt.plot(history.history['accuracy'], label='Training Accuracy')
        plt.plot(history.history['val_accuracy'], label='Validation Accuracy')
        plt.title('Accuracy Over Epochs')
        plt.xlabel('Epoch')
        plt.ylabel('Accuracy')
        plt.legend()
        plt.tight_layout()
        plt.savefig('training_history.png')
        plt.close()


    def predict(self, image_path, tile_size=512, overlap=64, save_path=None, save_viz=True, save_enhanced=True):

        # Load the original image
        original_img = cv2.imread(image_path)
        if original_img is None:
            raise ValueError(f"Cannot load image: {image_path}")

        original_img = cv2.cvtColor(original_img, cv2.COLOR_BGR2RGB)
        original_height, original_width = original_img.shape[:2]

        # Check if the image is small enough to process directly
        if original_height <= tile_size and original_width <= tile_size:
            # Predict directly without tiling
            print(f"Image size ({original_width}x{original_height}) is smaller than tile size ({tile_size}x{tile_size}). Predicting without tiling")
            with tf.device('/CPU:0'):
                img, _ = self.preprocess_image(image_path)
                img_batch = np.expand_dims(img, axis=0)
                prediction = self.model.predict(img_batch)[0]
        
                # Resize the prediction to original dimensions if necessary
                if self.resize_for_model and (prediction.shape[0] != original_height or prediction.shape[1] != original_width):
                    prediction_resized = cv2.resize(
                        prediction, (original_width, original_height),
                        interpolation=cv2.INTER_LINEAR
                    )
                else:
                    prediction_resized = prediction
        
                # process the prediction
                binary_prediction = (prediction_resized > 0.5).astype(np.uint8) * 255

                # Visualize the prediction
                self._visualize_prediction(image_path, prediction_resized, save_path, save_viz)
            
            #  Check if the original image is smaller than the tile size
            resized_img = cv2.resize(original_img, (self.img_width, self.img_height)) / 255.0 if self.resize_for_model else original_img / 255.0
            self._visualize_enhanced_prediction(image_path, resized_img, prediction_resized, binary_prediction, save_path, save_enhanced)
            
        else:
            # Tile-based prediction
            print(f"Image size ({original_width}x{original_height}) is larger than tile size ({tile_size}x{tile_size}). Using tiled prediction")
    
            prediction_map = np.zeros((original_height, original_width), dtype=np.float32)
            weight_map = np.zeros((original_height, original_width), dtype=np.float32)
            effective_tile_size = tile_size - 2 * overlap
    
            n_tiles_x = max(1, math.ceil((original_width - tile_size) / effective_tile_size) + 1)
            n_tiles_y = max(1, math.ceil((original_height - tile_size) / effective_tile_size) + 1)
    
            print(f"Tile Count: {n_tiles_x}x{n_tiles_y} = {n_tiles_x * n_tiles_y}")
    
            with tf.device('/CPU:0'):
                # Predict each tile
                for i in range(n_tiles_y):
                    for j in range(n_tiles_x):
                        y_start = min(i * effective_tile_size, max(0, original_height - tile_size))
                        x_start = min(j * effective_tile_size, max(0, original_width - tile_size))
                
                        tile = original_img[y_start:y_start + tile_size, x_start:x_start + tile_size]
                
                        tile_normalized = tile / 255.0
                
                        h, w = tile.shape[:2]
                        if h < tile_size or w < tile_size:
                            padded_tile = np.zeros((tile_size, tile_size, 3), dtype=np.float32)
                            padded_tile[:h, :w, :] = tile_normalized
                            tile_normalized = padded_tile
                
                        tile_batch = np.expand_dims(tile_normalized, axis=0)
                        tile_prediction = self.model.predict(tile_batch, verbose=0)[0]  # 첫 번째 차원 제거
                
                        weight = np.ones((tile_size, tile_size), dtype=np.float32)
                
                        if overlap > 0:
                            for k in range(overlap):
                                # Reduce the weight for top and bottom edges
                                weight[k, :] = weight[k, :] * (k + 1) / (overlap + 1)
                                weight[tile_size - 1 - k, :] = weight[tile_size - 1 - k, :] * (k + 1) / (overlap + 1)
                        
                                # Reduce the weight for left and right edges 
                                weight[:, k] = weight[:, k] * (k + 1) / (overlap + 1)
                                weight[:, tile_size - 1 - k] = weight[:, tile_size - 1 - k] * (k + 1) / (overlap + 1)
                
                        weighted_tile_prediction = tile_prediction[:, :, 0] * weight
                
                        prediction_map[y_start:y_start + tile_size, x_start:x_start + tile_size] += weighted_tile_prediction
                        weight_map[y_start:y_start + tile_size, x_start:x_start + tile_size] += weight
        
                weight_map = np.maximum(weight_map, 1e-10)
                prediction_map = prediction_map / weight_map
        
                # Process the prediction map
                binary_prediction = (prediction_map > 0.5).astype(np.uint8) * 255
        
                print(f"Prediction range: {prediction_map.min()} ~ {prediction_map.max()}")

                # Visualize the prediction
                self._visualize_prediction(image_path, prediction_map, save_path, save_viz)

            self._visualize_enhanced_prediction(image_path, original_img / 255.0, prediction_map, binary_prediction, save_path, save_enhanced)

        return binary_prediction

    def _visualize_prediction(self, image_path, prediction, save_path=None, save_viz=True):
        if not save_viz:
            return
            
        original_img = cv2.imread(image_path)
        original_img = cv2.cvtColor(original_img, cv2.COLOR_BGR2RGB)
        
        plt.figure(figsize=(12, 5))
        
        plt.subplot(1, 2, 1)
        plt.imshow(original_img)
        plt.title('Original Image')
        plt.axis('off')
        
        plt.subplot(1, 2, 2)
        plt.imshow(prediction, cmap='gray')
        plt.title('Prediction Mask')
        plt.axis('off')
        
        filename = os.path.basename(image_path)
        base_name = os.path.splitext(filename)[0]
        
        plt.tight_layout()
        output_path = os.path.join(save_path, f'visualization_{base_name}.png') if save_path else f'visualization_{base_name}.png'
        plt.savefig(output_path)
        plt.close()
    
    def _visualize_enhanced_prediction(self, image_path, original_img, prediction_raw, binary_prediction, save_path=None, save_enhanced=True):
        if not save_enhanced:
            return
            
        plt.figure(figsize=(15, 5))
        
        filename = os.path.basename(image_path)
        base_name = os.path.splitext(filename)[0]
        
        plt.subplot(1, 3, 1)
        plt.imshow(original_img)
        plt.title('Original image')
        plt.axis('off')
        
        plt.subplot(1, 3, 2)
        plt.imshow(prediction_raw, cmap='jet')
        plt.title('Raw Prediction (Probability)')
        plt.colorbar(fraction=0.046, pad=0.04)
        plt.axis('off')
        
        plt.subplot(1, 3, 3)
        plt.imshow(binary_prediction, cmap='gray')
        plt.title('Final Binary Mask')
        plt.axis('off')
        
        plt.tight_layout()
        output_path = os.path.join(save_path, f'enhanced_viz_{base_name}.png') if save_path else f'enhanced_viz_{base_name}.png'
        plt.savefig(output_path)
        plt.close()
            
    def save_model(self, model_path):
        if self.model is not None:
            self.model.save(model_path)
            print(f"Model saved to {model_path}")
        else:
            print("No model to save. Please train the model first!")

    def load_model(self, model_path):
        if os.path.exists(model_path):
            with tf.device('/CPU:0'):
                self.model = load_model(model_path)
                print(f"Model loaded from {model_path}")
        else:
            print(f"Model file not found at {model_path}")

    def evaluate_model(self, image_paths, mask_paths):
        if self.model is None:
            print("Model not loaded")
            return
        
            
        with tf.device('/CPU:0'):
            X, y = self.prepare_training_data(image_paths, mask_paths)
            loss, accuracy = self.model.evaluate(X, y)
            print(f"Loss: {loss:.4f}")
            print(f"Test accuracy: {accuracy:.4f}")

def collect_dataset(original_dir, binary_dir, extensions=('.jpg', '.png', '.jpeg')):
    image_paths = []
    mask_paths = []
    
    if not os.path.exists(original_dir):
        print(f"Source folder not found: {original_dir}")
        return image_paths, mask_paths
    
    if not os.path.exists(binary_dir):
        print(f"Binary mask folder not found: {binary_dir}")
        return image_paths, mask_paths
    
    for filename in os.listdir(original_dir):
        if filename.lower().endswith(extensions):
            image_path = os.path.join(original_dir, filename)
            mask_path = os.path.join(binary_dir, filename)
            
            if os.path.exists(mask_path):
                image_paths.append(image_path)
                mask_paths.append(mask_path)
            else:
                print(f"Warning - Mask not found -> {filename}")
    
    return image_paths, mask_paths

def check_mask_quality(mask_paths):
    print("Checking mask image...")
    
    has_binary_masks = True
    for mask_path in mask_paths:
        mask = cv2.imread(mask_path, cv2.IMREAD_GRAYSCALE)
        unique_values = np.unique(mask)
        
        # Check if the mask is binary (0 and 255)
        if len(unique_values) > 2 or (len(unique_values) == 2 and not (0 in unique_values and 255 in unique_values)):
            print(f"Warning: non-binary image -> {mask_path}, unique value: {unique_values}")
            has_binary_masks = False
    
    return has_binary_masks

def ProcessFolder(input_folder, output_folder=None, enhanced_output_folder=None, save_viz=True, save_enhanced=True):
    batch_size = 2
    print(f"Selected batch size: {batch_size}")

    trained_model_path = 'trained_Model.keras'

    if output_folder is not None:
        os.makedirs(output_folder, exist_ok=True)
    if enhanced_output_folder is not None:
        os.makedirs(enhanced_output_folder, exist_ok=True)

    # Initialize the binarizer
    img_height, img_width = 512, 512
    binarizer = DuckweedBinarizer(img_height=img_height, img_width=img_width)
    binarizer.build_model()

    if not os.path.exists(trained_model_path):
        print(f"Error: '{trained_model_path}' file not found")
        return None

    print(f"'{trained_model_path}' loading and analyzing")
    binarizer.load_model(trained_model_path)

    # Collected image files
    image_files = sorted(glob.glob(os.path.join(input_folder, "*.*")))
    if not image_files:
        print("Not image file found")
        return None

    results = []
    for img_path in image_files:
        if isStop:
            print("Processing paused by user")
            break

        result = ProcessImage(img_path, 
                            output_folder, 
                            enhanced_output_folder,
                            save_viz=save_viz, 
                            save_enhanced=save_enhanced,
                            binarizer=binarizer)
        if result is not None:
            results.append(result)

    print(f"Processing completed. Total {len(results)} images processed")
    return results

def ProcessImage(image_path, output_folder=None, enhanced_output_folder=None, 
                save_viz=True, save_enhanced=True, binarizer=None):
    import matplotlib
    import time
    matplotlib.use('Agg')  # Use non-interactive backend for matplotlib
    if binarizer is None:
        # Load the model if not provided
        img_height, img_width = 512, 512
        binarizer = DuckweedBinarizer(img_height=img_height, img_width=img_width)
        binarizer.build_model()
        
        trained_model_path = 'trained_Model.keras'
        if not os.path.exists(trained_model_path):
            print(f"Error: '{trained_model_path}' not found")
            return None
            
        print(f"'{trained_model_path}' loading and predicting")
        binarizer.load_model(trained_model_path)

    print(f"Predicting image: {image_path}")

    start_time = time.time()  # Start time for processing1

    try:
        result = binarizer.predict(image_path, 
                                 save_path=enhanced_output_folder,
                                 save_viz=save_viz, 
                                 save_enhanced=save_enhanced)

        if result is not None and output_folder is not None:
            filename = os.path.splitext(os.path.basename(image_path))[0] + '.png'  # 확장자를 .png로 변경
            output_path = os.path.join(output_folder, filename)
            cv2.imwrite(output_path, result)
            print(f"Prediction results saved: {output_path}")

        # Compute elapsed time
        elapsed_time = time.time() - start_time
        print(f"Image processing time: {elapsed_time:.2f} s")

        # Report the processing time to a CSV file
        report_path = os.path.join(output_folder if output_folder else ".", "analysisTimeReport.csv")
        with open(report_path, "a") as report_file:
            report_file.write(f"{os.path.basename(image_path)},{elapsed_time:.2f}\n")

        return result
    except Exception as e:
        print(f"Error processing image {image_path}: {e}")
        return None
    finally:
        import matplotlib.pyplot as plt
        plt.close('all')


class StopTrainingCallback(Callback):
    def __init__(self, should_stop_func):
        super().__init__()
        self.should_stop = should_stop_func
    
    def on_epoch_begin(self, epoch, logs=None):
        if self.should_stop():
            print(f"\nProcessing paused by user (Epoch {epoch + 1})")
            self.model.stop_training = True
    
    def on_batch_begin(self, batch, logs=None):
        if self.should_stop():
            print(f"\nProcessing paused by user")
            self.model.stop_training = True



if __name__ == "__main__":
    app = SIPEREAImageAnalyzer()
    def train(self, image_paths, mask_paths, validation_split=0.2, epochs=100, batch_size=8, should_stop=lambda: False, es_patience=30, lr_patience=10, k_folds=5):
        """
        Train the model with k-fold cross validation
        
        Parameters:
        -----------
        image_paths : list
            List of paths to training images
        mask_paths : list
            List of paths to corresponding mask images
        validation_split : float
            Fraction of data to use for validation when not using cross-validation
        epochs : int
            Maximum number of training epochs
        batch_size : int
            Batch size for training
        should_stop : callable
            Function that returns True if training should be stopped
        es_patience : int
            Patience for early stopping
        lr_patience : int
            Patience for learning rate reduction
        k_folds : int
            Number of folds for cross-validation (default: 5)
        """
        from sklearn.model_selection import KFold
        
        if self.model is None:
            self.build_model()
        
        with tf.device('/CPU:0'):
            X, y = self.prepare_training_data(image_paths, mask_paths)
            
            # Initialize K-fold cross-validation
            kf = KFold(n_splits=k_folds, shuffle=True, random_state=42)
            fold_histories = []
            fold_metrics = []
            
            # Perform k-fold cross-validation
            for fold, (train_idx, val_idx) in enumerate(kf.split(X)):
                print(f"\n--------- Training Fold {fold+1}/{k_folds} ---------")
                
                # Reset model for each fold
                tf.keras.backend.clear_session()
                self.build_model()
                
                # Split data for this fold
                X_train, X_val = X[train_idx], X[val_idx]
                y_train, y_val = y[train_idx], y[val_idx]
                print(f"Fold {fold+1} split: Training images = {len(X_train)}, Validation images = {len(X_val)}")
                
                # Setup data augmentation
                seed = 42
                data_gen_args = dict(
                    rotation_range=15,
                    width_shift_range=0.1,
                    height_shift_range=0.1,
                    shear_range=0.05,
                    zoom_range=[0.9, 1.1],
                    horizontal_flip=True,
                    fill_mode='nearest')

                image_datagen = ImageDataGenerator(**data_gen_args)
                mask_datagen = ImageDataGenerator(**data_gen_args)
                
                image_generator = image_datagen.flow(X_train, batch_size=batch_size, seed=seed)
                mask_generator = mask_datagen.flow(y_train, batch_size=batch_size, seed=seed)
                
                def train_generator():
                    while True:
                        if should_stop(): 
                            raise StopIteration("Training stopped by user")
                        X_batch = next(image_generator)
                        y_batch = next(mask_generator)
                        yield X_batch, y_batch
                        
                train_steps = (len(X_train) + batch_size - 1) // batch_size
                
                # Setup callbacks
                callbacks = [
                    EarlyStopping(monitor='val_loss', patience=es_patience, restore_best_weights=True, verbose=1),
                    ModelCheckpoint(f'model_fold_{fold+1}.keras', monitor='val_loss', save_best_weights=True, verbose=1),
                    ReduceLROnPlateau(monitor='val_loss', factor=0.5, patience=lr_patience, min_lr=1e-6, verbose=1),
                    StopTrainingCallback(should_stop)
                ]
                
                try:
                    # Train model for this fold
                    history = self.model.fit(
                        train_generator(),
                        steps_per_epoch=train_steps,
                        validation_data=(X_val, y_val),
                        epochs=epochs,
                        callbacks=callbacks,
                        verbose=1
                    )
                    
                    # Evaluate model on validation data
                    val_loss, val_accuracy = self.model.evaluate(X_val, y_val, verbose=1)
                    fold_metrics.append({
                        'fold': fold+1,
                        'val_loss': val_loss,
                        'val_accuracy': val_accuracy
                    })
                    
                    # Store history and model for this fold
                    fold_histories.append(history)
                    self.model.save(f'model_fold_{fold+1}.keras')
                    print(f"Model for fold {fold+1} saved as 'model_fold_{fold+1}.keras'")
                    
                except StopIteration:
                    print(f"\nTraining stopped by user during fold {fold+1}")
                    break
                    
            # Summarize cross-validation results
            if fold_metrics:
                print("\n========== Cross-Validation Results ==========")
                for metric in fold_metrics:
                    print(f"Fold {metric['fold']}: Loss = {metric['val_loss']:.4f}, Accuracy = {metric['val_accuracy']:.4f}")
                    
                # Calculate average metrics
                avg_val_loss = np.mean([m['val_loss'] for m in fold_metrics])
                avg_val_accuracy = np.mean([m['val_accuracy'] for m in fold_metrics])
                print(f"\nAverage: Loss = {avg_val_loss:.4f}, Accuracy = {avg_val_accuracy:.4f}")
                
                # Find best fold
                best_fold_idx = np.argmin([m['val_loss'] for m in fold_metrics])
                best_fold = fold_metrics[best_fold_idx]['fold']
                print(f"Best fold: {best_fold} with Loss = {fold_metrics[best_fold_idx]['val_loss']:.4f}")
                
                # Load the best model
                self.model = load_model(f'model_fold_{best_fold}.keras')
                print(f"Loaded best model from fold {best_fold}")
                
                # Save as final model
                self.model.save('bestTrained_Model.keras')
                print("Best model saved as 'bestTrained_Model.keras'")
                
                # Plot combined training history
                self._plot_cross_validation_history(fold_histories)
                
                return fold_histories
            else:
                print("No complete fold training available. Training was stopped prematurely.")
                return None

    def _plot_cross_validation_history(self, fold_histories):
        """Plot the training history across all folds"""
        plt.figure(figsize=(15, 6))
        
        # Plot loss
        plt.subplot(1, 2, 1)
        for i, history in enumerate(fold_histories):
            plt.plot(history.history['loss'], label=f'Train Loss (Fold {i+1})', alpha=0.7)
            plt.plot(history.history['val_loss'], label=f'Val Loss (Fold {i+1})', linestyle='--', alpha=0.7)
        plt.title('Loss Over Epochs (All Folds)')
        plt.xlabel('Epoch')
        plt.ylabel('Loss')
        plt.legend()
        
        # Plot accuracy
        plt.subplot(1, 2, 2)
        for i, history in enumerate(fold_histories):
            plt.plot(history.history['accuracy'], label=f'Train Acc (Fold {i+1})', alpha=0.7)
            plt.plot(history.history['val_accuracy'], label=f'Val Acc (Fold {i+1})', linestyle='--', alpha=0.7)
        plt.title('Accuracy Over Epochs (All Folds)')
        plt.xlabel('Epoch')
        plt.ylabel('Accuracy')
        plt.legend()
        
        plt.tight_layout()
        plt.savefig('cross_validation_history.png')
        plt.close()
        
        print("Cross-validation history plot saved as 'cross_validation_history.png'")
    app.mainloop()  