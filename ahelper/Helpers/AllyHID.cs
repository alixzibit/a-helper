using HidSharp;
using System.Diagnostics;

namespace ahelper.Helpers
{
    public static class AllyHID
    {
        public const int ASUS_ID = 0x0b05; // Vendor ID for Asus
        public const int ROG_ALLY_ID = 0x1abe; // Product ID for the ROG Ally device
        public const byte INPUT_ID = 0x5a; // Assuming the specific report ID for the input data

        static HidStream? rogAllyStream;

        public static HidDevice? FindDevice()
        {
            HidDeviceLoader loader = new HidDeviceLoader();
            try
            {
                var device = loader.GetDevices(ASUS_ID).FirstOrDefault(d =>
                    d.ProductID == ROG_ALLY_ID && d.CanOpen && d.GetMaxFeatureReportLength() > 0);
                return device;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error finding ROG Ally device: {ex.Message}");
                return null;
            }
        }

        public static HidStream? OpenHidStream()
        {
            try
            {
                var device = FindDevice();
                if (device is not null)
                {
                    Debug.WriteLine(
                        $"Opening HID stream for device: {device.DevicePath} {device.ProductID.ToString("X")}");
                    return device.Open();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening HID stream: {ex.Message}");
            }

            return null;
        }

        public static void WriteInput(byte[] data, string? log = "USB")
        {
            var device = FindDevice();
            if (device is null)
            {
                Debug.WriteLine("ROG Ally device not found.");
                return;
            }

            try
            {
                using (var stream = device.Open())
                {
                    var payload = new byte[device.GetMaxFeatureReportLength()];
                    Array.Copy(data, payload, data.Length);
                    stream.SetFeature(payload);
                    if (log is not null)
                        Debug.WriteLine(
                            $"{log} {device.ProductID.ToString("X")}|{device.GetMaxFeatureReportLength()}: {BitConverter.ToString(data)}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"Error setting feature for device {device.DevicePath}: {BitConverter.ToString(data)} {ex.Message}");
            }
        }
    }
}
