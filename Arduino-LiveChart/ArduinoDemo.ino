#define DEBUG(id, value) { Serial.print(id);  Serial.print(':');  Serial.println(value); }
#define DEBUG_WITH_TIMESTAMP(id, value) { unsigned long ms = millis(); Serial.print(id);  Serial.print(':');  Serial.print(value);  Serial.print('@');  Serial.println(ms); }
#define DEBUG_WITH_MILLIS(id, value, ms) { Serial.print(id);  Serial.print(':');  Serial.print(value);  Serial.print('@');  Serial.println(ms); }

void setup() {
  Serial.begin(115200);
}


void loop() {
    while(Serial.available())
    {
      Serial.print((char)Serial.read());
    }

    //DEBUG("NO_MILLIS", 2.0*cos(millis()/500.0));

    //unsigned long ms = millis();
    //DEBUG_WITH_MILLIS("WITH_MILLIS", 2.0*cos(ms/500.0), ms);

    DEBUG_WITH_TIMESTAMP("WITH_MILLIS", 2.0*cos(ms/500.0));

    //DEBUG("T1", 2.0*cos(millis()/500.0));
    //DEBUG("T2", 1.5*cos(millis()/1000.0));
    //DEBUG("T3", cos(millis()/5000.0));
    
    //Serial.print("T1:");
    //Serial.println();

    //Serial.print("T2:");
    //Serial.println(2.0*sin(millis()/1000.0));

    //Serial.print("T3:");
    //Serial.println(cos(millis()/5000.0));
    delay(100);
}
