// The flag signals to the rest of the program an interrupt occured
static volatile bool button_flag_jump = false;
static volatile bool button_flag_dash = false;
int buttonPinJump;
int buttonPinDash;
int ledPinJump;
int ledPinDash;
int potPin;

// Remember the state the river in the Unity program is in

// Interrupt handler, sets the flag for later processing
void ICACHE_RAM_ATTR buttonPressJump() {
  button_flag_jump = true;
}

void ICACHE_RAM_ATTR buttonPressDash() {
  button_flag_dash = true;
}

void setup() {
  buttonPinJump = D12;
  buttonPinDash = D13;
  ledPinJump = D8;
  ledPinDash = D9;
  potPin = A0;
  
  pinMode(ledPinJump, OUTPUT);
  pinMode(ledPinDash, OUTPUT);
  // Internal pullup, no external resistor necessary
  pinMode(buttonPinJump,INPUT_PULLUP);
  pinMode(buttonPinDash,INPUT_PULLUP);
  // 115200 is a common baudrate : fast without being overwhelming
  Serial.begin(115200);

  // As the button is in pullup, detect a connection to ground
  attachInterrupt(digitalPinToInterrupt(buttonPinJump),buttonPressJump,FALLING);
  attachInterrupt(digitalPinToInterrupt(buttonPinDash),buttonPressDash,FALLING);

  pinMode(potPin, INPUT);
}

// Processes button input
void loop() {
  // Slows reaction down a bit
  // but prevents _most_ button press misdetections
  delay(5);
  
  if (button_flag_jump) {
    Serial.println("JUMP");
    button_flag_jump = false;
  }
  if (button_flag_dash) {
    Serial.println("DASH");
    button_flag_dash = false;
  }

  int potentiometre = analogRead(potPin);
  //Serial.println(potentiometre);
}

// Handles incoming messages
// Called by Arduino if any serial data has been received
void serialEvent()
{
  String message = Serial.readStringUntil('\r');
  Serial.println(message);
  if (message == "3") {
    digitalWrite(ledPinDash, HIGH);
    digitalWrite(ledPinJump, HIGH);
  } else if (message == "2") {
    digitalWrite(ledPinDash, HIGH);
    digitalWrite(ledPinJump, LOW);
  } else if (message == "1") {
    digitalWrite(ledPinJump, HIGH);
    digitalWrite(ledPinDash, LOW);
  } else if (message == "0") {
    digitalWrite(ledPinJump, LOW);
    digitalWrite(ledPinDash, LOW);
  }
}
