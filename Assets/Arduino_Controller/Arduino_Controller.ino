// The flag signals to the rest of the program an interrupt occured
static bool button_flag = false;
// Remember the state the river in the Unity program is in
static bool river_state = false;

// Interrupt handler, sets the flag for later processing
void ICACHE_RAM_ATTR buttonPress() {
  button_flag = true;
}

void ICACHE_RAM_ATTR switchChange() {
  Serial.println("DAY " + String(digitalRead(D6)));
}

void setup() {
  int buttonPin = D4;
  
  pinMode(LED_BUILTIN, OUTPUT);
  // Internal pullup, no external resistor necessary
  pinMode(buttonPin,INPUT_PULLUP);
  // 115200 is a common baudrate : fast without being overwhelming
  Serial.begin(115200);

  // As the button is in pullup, detect a connection to ground
  attachInterrupt(digitalPinToInterrupt(buttonPin),buttonPress,FALLING);

  pinMode(A0, INPUT);

  pinMode(D6, INPUT);
  attachInterrupt(digitalPinToInterrupt(D6), switchChange, CHANGE);
}

// Processes button input
void loop() {
  // Slows reaction down a bit
  // but prevents _most_ button press misdetections
  delay(200);
  
  if (button_flag) {
    if (river_state) {
      Serial.println("dry");
    } else {
      Serial.println("wet");
    }
    river_state = !river_state;
    button_flag = false;
  }

  int potentiometre = analogRead(A0);
  Serial.println(potentiometre);

  
}

// Handles incoming messages
// Called by Arduino if any serial data has been received
void serialEvent()
{
  String message = Serial.readStringUntil('\r');
  if (message == "LED ON") {
    digitalWrite(LED_BUILTIN, LOW);
  } else if (message == "LED OFF") {
    digitalWrite(LED_BUILTIN, HIGH);
  }
}
