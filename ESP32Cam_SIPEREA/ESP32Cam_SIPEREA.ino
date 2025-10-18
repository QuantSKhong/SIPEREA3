#include "Arduino.h"
#include "esp_camera.h"
#include <WiFi.h>

//-----------------------------------------------------
const String programLabel = "ESP32Cam_SIPEREA"; 
const String programDate  = "2024-06-22";

const int deviceID = 103;
const int pinRelay = 12;
const int pinLED = 4;

const char* ssid = "sk-ip3";
const char* password = "74256979";
// Set your Static IP address
String local_IP_prefix = "192.168.137.";
IPAddress local_IP(192, 168, 137, deviceID);
IPAddress gateway(192, 168, 137, 1);
IPAddress subnet(255, 255, 255, 0); 


//const char* ssid = "sk-ip1";
//const char* password = "74256979";
//// Set your Static IP address
//String local_IP_prefix = "192.168.137.";
//IPAddress local_IP(192, 168, 137, deviceID);
//IPAddress gateway(192, 168, 137, 1);
//IPAddress subnet(255, 255, 255, 0); 


//-----------------------------------------------------
 
#define PWDN_GPIO_NUM     32
#define RESET_GPIO_NUM    -1
#define XCLK_GPIO_NUM      0
#define SIOD_GPIO_NUM     26
#define SIOC_GPIO_NUM     27
 
#define Y9_GPIO_NUM       35
#define Y8_GPIO_NUM       34
#define Y7_GPIO_NUM       39
#define Y6_GPIO_NUM       36
#define Y5_GPIO_NUM       21
#define Y4_GPIO_NUM       19
#define Y3_GPIO_NUM       18
#define Y2_GPIO_NUM        5
#define VSYNC_GPIO_NUM    25
#define HREF_GPIO_NUM     23
#define PCLK_GPIO_NUM     22
 
#include "esp_system.h"
const int wdtTimeout = 30000;  //time in ms to trigger the watchdog
hw_timer_t *timer = NULL;
void IRAM_ATTR resetModule() {
    ets_printf("reboot\n");
    esp_restart();
}
 
 
#include "soc/soc.h"           // Disable brownour problems
#include "soc/rtc_cntl_reg.h"  // Disable brownour problems
 

 
void startCameraServer(); 
 
void setup() {
      setCpuFrequencyMhz(240);
      
      WRITE_PERI_REG(RTC_CNTL_BROWN_OUT_REG, 0); //disable brownout detector
 
      Serial.begin(115200); 

      Serial.print(F("Program : "));
      Serial.println(programLabel);
      delay(5);
      Serial.print(F("Version : "));
      Serial.println(programDate);
      delay(5);
      
      camera_config_t config;
      config.ledc_channel = LEDC_CHANNEL_0;
      config.ledc_timer = LEDC_TIMER_0;
      config.pin_d0 = Y2_GPIO_NUM;
      config.pin_d1 = Y3_GPIO_NUM;
      config.pin_d2 = Y4_GPIO_NUM;
      config.pin_d3 = Y5_GPIO_NUM;
      config.pin_d4 = Y6_GPIO_NUM;
      config.pin_d5 = Y7_GPIO_NUM;
      config.pin_d6 = Y8_GPIO_NUM;
      config.pin_d7 = Y9_GPIO_NUM;
      config.pin_xclk = XCLK_GPIO_NUM;
      config.pin_pclk = PCLK_GPIO_NUM;
      config.pin_vsync = VSYNC_GPIO_NUM;
      config.pin_href = HREF_GPIO_NUM;
      config.pin_sscb_sda = SIOD_GPIO_NUM;
      config.pin_sscb_scl = SIOC_GPIO_NUM;
      config.pin_pwdn = PWDN_GPIO_NUM;
      config.pin_reset = RESET_GPIO_NUM;
      config.xclk_freq_hz = 20000000;
      config.pixel_format = PIXFORMAT_JPEG;
 
      if(psramFound()){
          config.frame_size = FRAMESIZE_UXGA;
          config.jpeg_quality = 10;
          config.fb_count = 2;
          
      } else {
          config.frame_size = FRAMESIZE_SVGA;
          config.jpeg_quality = 12;
          config.fb_count = 1;
          config.grab_mode = CAMERA_GRAB_LATEST;
      }
      
      config.grab_mode = CAMERA_GRAB_LATEST;
      
       // camera init
      esp_err_t err = esp_camera_init(&config);
      if (err != ESP_OK) {
        Serial.printf("Camera init failed with error 0x%x", err);
        return;
      }
 
      sensor_t * s = esp_camera_sensor_get();
      s->set_brightness(s, 0);     // -2 to 2
      s->set_contrast(s, 0);       // -2 to 2
      s->set_saturation(s, 0);     // -2 to 2
      s->set_special_effect(s, 0); // 0 to 6 (0 - No Effect, 1 - Negative, 2 - Grayscale, 3 - Red Tint, 4 - Green Tint, 5 - Blue Tint, 6 - Sepia)
      s->set_whitebal(s, 1);       // 0 = disable , 1 = enable
      s->set_awb_gain(s, 1);       // 0 = disable , 1 = enable
      s->set_wb_mode(s, 0);        // 0 to 4 - if awb_gain enabled (0 - Auto, 1 - Sunny, 2 - Cloudy, 3 - Office, 4 - Home)
      s->set_exposure_ctrl(s, 1);  // 0 = disable , 1 = enable
      s->set_aec2(s, 1);           // 0 = disable , 1 = enable
      s->set_ae_level(s, 0);       // -2 to 2
      s->set_aec_value(s, 300);    // 0 to 1200
      s->set_gain_ctrl(s, 1);      // 0 = disable , 1 = enable
      s->set_agc_gain(s, 0);       // 0 to 30
      s->set_gainceiling(s, (gainceiling_t)0);  // 0 to 6
      s->set_bpc(s, 0);            // 0 = disable , 1 = enable
      s->set_wpc(s, 1);            // 0 = disable , 1 = enable
      s->set_raw_gma(s, 1);        // 0 = disable , 1 = enable
      s->set_lenc(s, 1);           // 0 = disable , 1 = enable
      s->set_hmirror(s, 1);        // 0 = disable , 1 = enable
      s->set_vflip(s, 0);          // 0 = disable , 1 = enable
      s->set_dcw(s, 1);            // 0 = disable , 1 = enable
      s->set_colorbar(s, 0);       // 0 = disable , 1 = enable

 
      timer = timerBegin(0, 80, true);                  //timer 0, div 80
      timerAttachInterrupt(timer, &resetModule, true);  //attach callback
      timerAlarmWrite(timer, wdtTimeout * 1000, false); //set time in us
      timerAlarmEnable(timer);                          //enable interrupt 


      pinMode(pinRelay, OUTPUT);
      digitalWrite(pinRelay, LOW);
      //pinMode(pinLED, OUTPUT);
      //digitalWrite(pinLED, LOW);
      
}
 
void loop() {
      timerWrite(timer, 0); //reset timer (feed watchdog)
       
      if (WiFi.status() != WL_CONNECTED) {
          connectToWifi();
      }

      for (int t=0; t<60; t++) {
          timerWrite(timer, 0);
          delay(1000);
      }
}
 
 
void connectToWifi() {
      // Configures static IP address
      if (!WiFi.config(local_IP, gateway, subnet)) {      
          Serial.println("STA Failed to configure");
          delay(5);
          return;
      }    
 
      WiFi.begin(ssid, password);

      for (int t=0; t<20; t++) {
          timerWrite(timer, 0);          
          delay(1000);
          Serial.print("."); 
          
          if(WiFi.status() == WL_CONNECTED) {
              Serial.println("");
              Serial.println("WiFi connected");
              delay(5);
         
              startCameraServer();
         
              Serial.println("Your camera is ready to stream at");
              delay(5);
              Serial.print("http://");
              delay(5);
              Serial.println(WiFi.localIP());
              delay(5);
                  
              pinMode(pinLED, OUTPUT);
              digitalWrite(pinLED, HIGH);
       
              if (WiFi.localIP().toString() == local_IP_prefix + String(deviceID)) {
                    blinkLED(1);   
              } else {
                    blinkLED(3);
              }
              return;
          }
      }

      Serial.println("");
      Serial.println("WiFi connection failed");
      delay(5);            
}

void blinkLED(int count) {
    for (int q=0; q<count; q++ ) {
        digitalWrite(pinLED, HIGH);   
        delay(100);
        digitalWrite(pinLED, LOW);   
        delay(900);
    }  
}
