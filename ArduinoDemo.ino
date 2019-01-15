#define LIVESERIAL(id, value) { Serial.print(id);  Serial.print(':');  Serial.println(value); }
#define LIVESERIAL_MILLIS(id, value) { unsigned long ms = millis(); Serial.print(id);  Serial.print(':');  Serial.print(value);  Serial.print('@');  Serial.println(ms); }

void setup() {
  Serial.begin(115200);
}

void loop() {
    while(Serial.available())
    {
      Serial.print((char)Serial.read());
    }

    LIVESERIAL("NORMAL", 2.0*cos(millis()/500.0));
    LIVESERIAL_MILLIS("WITH_MILLIS", 2.0*cos(ms/500.0));
    delay(100);
}
