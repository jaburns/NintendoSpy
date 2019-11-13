
void setup() {
  Serial.begin(115200);
  Serial1.begin(9600, SERIAL_8E1);
}

void loop() {
  int incomingByte;
        
  if (Serial1.available() > 0) 
  {
    incomingByte = Serial1.read();
    Serial.print("UART received: ");
    Serial.println(incomingByte, DEC);
  }
}
