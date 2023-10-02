using System;
using SharpDX.DirectInput;

class Program
{
    static void Main()
    {
        // DirectInputのインスタンスを作成
        var directInput = new DirectInput();

        // 接続されているすべてのジョイスティックを取得
        var joystickGuids = directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices);
        var joystick = FindG29(directInput);

        if (joystick == null)
        {
            Console.WriteLine("G29が見つかりませんでした。");
            return;
        }

        joystick.Acquire();

        while (true)
        {
            joystick.Poll();
            var joystickState = joystick.GetCurrentState();

            // ハンドルの角度 (通常はX軸)
            int wheelPosition = joystickState.X;

            // アクセルの開度 (通常はY軸)
            int acceleratorPosition = joystickState.Y;

            // クラッチの開度 (RotationZを使用する場合)
            int clutchPosition = joystickState.Sliders[0];

            // ブレーキの開度
            int brakePosition = joystickState.RotationZ;

            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            Console.WriteLine($"{currentTime} - Wheel: {wheelPosition}, Accelerator: {acceleratorPosition}, Brake: {brakePosition}, Clutch: {clutchPosition}");

            //System.Threading.Thread.Sleep(1);
        }
    }

    static Joystick? FindG29(DirectInput directInput)  // static を追加
    {
        foreach (var deviceType in new[] { DeviceType.Driving, DeviceType.Gamepad, DeviceType.Joystick })
        {
            foreach (var deviceInstance in directInput.GetDevices(deviceType, DeviceEnumerationFlags.AllDevices))
            {
                if (deviceInstance.ProductName.Contains("G29"))
                {
                    return new Joystick(directInput, deviceInstance.InstanceGuid);
                }
            }
        }
        return null;
    }
}
