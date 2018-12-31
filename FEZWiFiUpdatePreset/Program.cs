// RoSchmi 31.12.2018, License: MIT
// Utility to bring GHI FEZ Board in the right 
// condition for SPWF04SA Firmware Update

// How to use:
// Connect FTDI Adapter and connect the H1 pin to 3.3 V (best through a 10 kOhms resistor)
// Start the Program and wait for message: "Ready to reset Uart - Press a button..."
// Press BTN1 Button
//
// Then repower the board an you are ready to update the SPWF04SA Firmware

using System;
using System.Collections;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using System.Diagnostics;

namespace FEZWiFiUpdatePreset
{
    class Program
    {
        private static GpioPin _pinReset;
        private static GpioPin _pinUart;
        private static GpioPin _pinPyton;
        private static GpioPin _pinLed;
        private static GpioPin _pinButton;

        static void Main()
        {
            System.Diagnostics.Debug.WriteLine("Hello FEZ");
            var ctl = GpioController.GetDefault();

            _pinPyton = ctl.OpenPin(FEZCLR.GpioPin.PA0);
            _pinPyton.SetDriveMode(GpioPinDriveMode.InputPullDown);

            _pinButton = ctl.OpenPin(FEZ.GpioPin.Btn1);
            _pinButton.SetDriveMode(GpioPinDriveMode.InputPullUp);

            _pinUart = ctl.OpenPin(FEZCLR.GpioPin.PA1);
            _pinUart.SetDriveMode(GpioPinDriveMode.Output);
            _pinUart.Write(GpioPinValue.Low);
            Thread.Sleep(5);

            _pinReset = ctl.OpenPin(FEZ.GpioPin.WiFiReset);
            _pinReset.SetDriveMode(GpioPinDriveMode.Output);
            _pinReset.Write(GpioPinValue.High);
            Thread.Sleep(10);

            _pinLed = ctl.OpenPin(FEZ.GpioPin.Led1);
            _pinLed.SetDriveMode(GpioPinDriveMode.Output);
            _pinLed.Write(GpioPinValue.High);

            // Indicates that it's working
            Thread.Sleep(5000);
            _pinLed.Write(GpioPinValue.Low);
            Thread.Sleep(2000);
            WaitForButton("Ready to reset Uart - Press a button...");
            ResetToUart();
            while (true)
            {
                _pinLed.Write(_pinLed.Read() == GpioPinValue.Low ? GpioPinValue.High : GpioPinValue.Low);
                Thread.Sleep(100);
            }
        }

        private static void ResetToUart()
        {           
            _pinReset.Write(GpioPinValue.Low);
            Thread.Sleep(20); //Generous wait
            _pinReset.Write(GpioPinValue.High);
        }

        private static void WaitForButton(string msg = "")
        {
            if (!String.IsNullOrEmpty(msg))
                Debug.WriteLine(msg);
            while (_pinButton.Read() == GpioPinValue.High)
            {
                _pinLed.Write(_pinLed.Read() == GpioPinValue.High ? GpioPinValue.Low : GpioPinValue.High);
                Thread.Sleep(50);
            }

            while (_pinButton.Read() == GpioPinValue.Low)
                Thread.Sleep(50);
        }
    }
}
