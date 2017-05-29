#include <Wire.h>
#include <SoftwareSerial.h>



SoftwareSerial bluetoothSerial(4, 2); // RX, TX



int tmp102Address = 0x48;//pentru adresa senzorului de tempertura

volatile int rate[10];                    // array to hold last ten IBI values
volatile unsigned long sampleCounter = 0;          // used to determine pulse timing
volatile unsigned long lastBeatTime = 0;           // used to find IBI
volatile int P =512;                      // used to find peak in pulse wave, seeded
volatile int T = 512;                     // used to find trough in pulse wave, seeded
volatile int thresh = 512;                // used to find instant moment of heart beat, seeded
volatile int amp = 100;                   // used to hold amplitude of pulse waveform, seeded
volatile boolean firstBeat = true;        // used to seed rate array so we startup with reasonable BPM
volatile boolean secondBeat = false;      // used to seed rate array so we startup with reasonable BPM

//  VARIABLES
int pulsePin = 1;                 // Pulse Sensor purple wire connected to analog pin 0
int puls=1;                       //variabila pt BPM-ul afisat
int pinLOminus=8;                 //pin pentru EKG LO-
int pinLOplus=9;                  //pin pentru EKG LO+
int vectorEkg[41];



// these variables are volatile because they are used during the interrupt service routine!
volatile int BPM;                   // used to hold the pulse rate
volatile int Signal;                // holds the incoming raw data
volatile int IBI = 600;             // holds the time between beats, must be seeded! 
volatile boolean Pulse = false;     // true when pulse wave is high, false when it's low
volatile boolean QS = false;        // becomes true when Arduoino finds a beat.



void setup(){
  Serial.begin(57600);
  Wire.begin();
  bluetoothSerial.begin(115200);           //pt comunicatia prin bluetooth
   delay(320);
   bluetoothSerial.print("$$$");         // Enter command mode
  delay(15);                      // IMPORTANT DELAY! (Minimum ~10ms)
  bluetoothSerial.println("U,9600,N");  // Temporarily Change the baudrate to 9600, no parity
  bluetoothSerial.begin(9600);
  //interruptSetup();                 // sets up to read Pulse Sensor signal every 2mS 
  pinMode(pinLOminus,INPUT);
  pinMode(pinLOplus,INPUT);
 // bluetooth.begin(115200);        // The Bluetooth Mate defaults to 115200bps
                      // IMPORTANT DELAY! (Minimum ~276ms)
  
}

void loop(){

  float celsius = getTemperature();
 // Serial.println("Celsius: ");
//  Serial.println(celsius);
   
  if (QS == true){                       // Quantified Self flag is true when arduino finds a heartbeat
     //   Serial.println("BPM: ");
     //   Serial.println(puls);         
        QS = false;                   // reset the Quantified Self flag for next time    
     }
 else
     {
   //   Serial.println("BPM: ");
    //  Serial.println("!");  
     }

  if((digitalRead(9)==1)||(digitalRead(8)==1))
   {
     // Serial.println('!');
    }
  
    else
    {
      for(int i=0;i<40;i++){
      vectorEkg[i]=analogRead(A0);
      delay(10); 
      }
    }

  String val_temperatura = String(celsius, 2);
  String val_puls = String(puls);
  String val_ecg = String(vectorEkg[0]);
  for(int i=1; i<99; i++)
  {
      val_ecg.concat(",");
      val_ecg.concat(String(vectorEkg[i]));
    }
 String resoult = "";
 resoult.concat("*T:");
 resoult.concat(val_temperatura);
 resoult.concat(";B:");
 resoult.concat(val_puls);
 resoult.concat(";E:");
 resoult.concat(val_ecg);
 resoult.concat(",...;");
  
  /*bluetoothSerial.print("*T:");
  bluetoothSerial.print(celsius);
  bluetoothSerial.print(";B:");
  bluetoothSerial.print(puls);
  bluetoothSerial.print(";E:");
  for(int i=0;i<99;i++)
  {
    bluetoothSerial.print(vectorEkg[i]);
    bluetoothSerial.print(",");
  }
  bluetoothSerial.print("...;");*/
  //bluetoothSerial.println(vectorEkg[39]);
  /*Serial.print("*T:");
  Serial.print(celsius);
  Serial.print(";B:");
  Serial.print(puls);
  Serial.print(";E:");*/
// for(int i=0;i<99;i++)
  //{
 //   Serial.print(vectorEkg[i]);
  // Serial.print(",");
  //}
  Serial.print(resoult);
  bluetoothSerial.print(resoult);
  delay(5000); //just here to slow down the output. You can remove this
}





float getTemperature(){
  Wire.requestFrom(tmp102Address,2); 

  byte MSB = Wire.read();
  byte LSB = Wire.read();

  //it's a 12bit int, using two's compliment for negative
  int TemperatureSum = ((MSB << 8) | LSB) >> 4; 

  float celsius = TemperatureSum*0.0625;
  return celsius;
}

/*---------------------------------------------------------------------------------------------*/

void interruptSetup(){     
  // Initializes Timer2 to throw an interrupt every 2mS.
  TCCR2A = 0x02;     // DISABLE PWM ON DIGITAL PINS 3 AND 11, AND GO INTO CTC MODE
  TCCR2B = 0x06;     // DON'T FORCE COMPARE, 256 PRESCALER 
  OCR2A = 0X7C;      // SET THE TOP OF THE COUNT TO 124 FOR 500Hz SAMPLE RATE
  TIMSK2 = 0x02;     // ENABLE INTERRUPT ON MATCH BETWEEN TIMER2 AND OCR2A
  sei();             // MAKE SURE GLOBAL INTERRUPTS ARE ENABLED      
} 


// THIS IS THE TIMER 2 INTERRUPT SERVICE ROUTINE. 
// Timer 2 makes sure that we take a reading every 2 miliseconds
ISR(TIMER2_COMPA_vect){                         // triggered when Timer2 counts to 124
  cli();                                      // disable interrupts while we do this
  Signal = analogRead(pulsePin);              // read the Pulse Sensor 
  sampleCounter += 2;                         // keep track of the time in mS with this variable
  int N = sampleCounter - lastBeatTime;       // monitor the time since the last beat to avoid noise

    //  find the peak and trough of the pulse wave
  if(Signal < thresh && N > (IBI/5)*3){       // avoid dichrotic noise by waiting 3/5 of last IBI
    if (Signal < T){                        // T is the trough
      T = Signal;                         // keep track of lowest point in pulse wave 
    }
  }

  if(Signal > thresh && Signal > P){          // thresh condition helps avoid noise
    P = Signal;                             // P is the peak
  }                                        // keep track of highest point in pulse wave

  //  NOW IT'S TIME TO LOOK FOR THE HEART BEAT
  // signal surges up in value every time there is a pulse
  if (N > 250){                                   // avoid high frequency noise
    if ( (Signal > thresh) && (Pulse == false) && (N > (IBI/5)*3) ){        
      Pulse = true;                               // set the Pulse flag when we think there is a pulse
      IBI = sampleCounter - lastBeatTime;         // measure time between beats in mS
      lastBeatTime = sampleCounter;               // keep track of time for next pulse

      if(secondBeat){                        // if this is the second beat, if secondBeat == TRUE
        secondBeat = false;                  // clear secondBeat flag
        for(int i=0; i<=9; i++){             // seed the running total to get a realisitic BPM at startup
          rate[i] = IBI;                      
        }
      }

      if(firstBeat){                         // if it's the first time we found a beat, if firstBeat == TRUE
        firstBeat = false;                   // clear firstBeat flag
        secondBeat = true;                   // set the second beat flag
        sei();                               // enable interrupts again
        return;                              // IBI value is unreliable so discard it
      }   


      // keep a running total of the last 10 IBI values
      word runningTotal = 0;                  // clear the runningTotal variable    

      for(int i=0; i<=8; i++){                // shift data in the rate array
        rate[i] = rate[i+1];                  // and drop the oldest IBI value 
        runningTotal += rate[i];              // add up the 9 oldest IBI values
      }

      rate[9] = IBI;                          // add the latest IBI to the rate array
      runningTotal += rate[9];                // add the latest IBI to runningTotal
      runningTotal /= 10;                     // average the last 10 IBI values 
      BPM = 60000/runningTotal;               // how many beats can fit into a minute? that's BPM!
      puls=BPM;
      QS = true;                              // set Quantified Self flag 
      // QS FLAG IS NOT CLEARED INSIDE THIS ISR
    }                       
  }

  if (Signal < thresh && Pulse == true){   // when the values are going down, the beat is over
   
    Pulse = false;                         // reset the Pulse flag so we can do it again
    amp = P - T;                           // get amplitude of the pulse wave
    thresh = amp/2 + T;                    // set thresh at 50% of the amplitude
    P = thresh;                            // reset these for next time
    T = thresh;
  }

  if (N > 2500){                           // if 2.5 seconds go by without a beat
    thresh = 512;                          // set thresh default
    P = 512;                               // set P default
    T = 512;                               // set T default
    lastBeatTime = sampleCounter;          // bring the lastBeatTime up to date        
    firstBeat = true;                      // set these to avoid noise
    secondBeat = false;                    // when we get the heartbeat back
  }

  sei();                                   // enable interrupts when youre done!
}// end isr







