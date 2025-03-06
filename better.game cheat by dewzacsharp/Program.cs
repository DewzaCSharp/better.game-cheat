using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

public class Program
{
    [DllImport("kernel32.dll")]
    static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

    [DllImport("kernel32.dll")]
    static extern void CloseHandle(IntPtr hObject);

    const int PROCESS_ALL_ACCESS = 0x1F0FFF;
    const uint PAGE_EXECUTE_READWRITE = 0x40;

    public static Dewz client = new Dewz("Protoverse");

    static void Patch(IntPtr hProcess, IntPtr address, byte[] bytes)
    {
        uint oldProtect;
        VirtualProtectEx(hProcess, address, bytes.Length, PAGE_EXECUTE_READWRITE, out oldProtect);
        WriteProcessMemory(hProcess, address, bytes, bytes.Length, out _);
        VirtualProtectEx(hProcess, address, bytes.Length, oldProtect, out _);
    }

    static void Main()
    {
        Console.Clear();
        Thread titlethread = new Thread(new Program().NiceTitle);
        titlethread.Start();
        string processName = "Protoverse";
        // Protoverse.wi::scene::CameraComponent::TransformCamera+199 - F2 44 0F11 4B 28 open unten
        // "Protoverse.exe"+282469

        // Protoverse.wi::scene::CameraComponent::TransformCamera+1A4 - F3 44 0F11 4B 30 links rechts
        // "Protoverse.exe"+282474

        // makes camera more fun Protoverse.wi::scene::CameraComponent::TransformCamera+1C5 - F2 0F11 7B 40
        // "Protoverse.exe"+282495

        // sideways camera Protoverse.wi::scene::CameraComponent::UpdateCamera+11A - F2 0F10 53 40
        // "Protoverse.exe"+281FDA

        // if nop cant reload Protoverse.update_projectiles+A27 - F2 0F10 0D E1BCC600
        // "Protoverse.exe"+185D27

        byte[] PumpAmmoOG = { 0x89, 0x83, 0x9C, 0x04, 0x00, 0x00 };
        byte[] PumpAmmo = { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 };

        byte[] arAmmoOG = { 0x89, 0x83, 0xAC, 0x04, 0x00, 0x00 };
        byte[] arAmmo = { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 };

        byte[] CameraUpDownOG = { 0xF2, 0x44, 0x0F, 0x11, 0x4B, 0x28 };
        byte[] CameraUpDownPatch = { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 };

        byte[] CameraLeftRightOG = { 0xF3, 0x44, 0x0F, 0x11, 0x4B, 0x30 };
        byte[] CameraLeftRightPatch = { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 };

        byte[] CameraFunOG = { 0xF2, 0x0F, 0x11, 0x7B, 0x40 };
        byte[] CameraFunPatch = { 0x90, 0x90, 0x90, 0x90, 0x90 };

        byte[] SidewaysCameraOG = { 0xF2, 0x0F, 0x10, 0x53, 0x40 };
        byte[] SidewaysCameraPatch = { 0x90, 0x90, 0x90, 0x90, 0x90 };

        while (Process.GetProcessesByName("Protoverse").Length == 0)
        {
            Console.WriteLine("Game is not running!");
            Thread.Sleep(1000);
            Console.Clear();
        }
        Process process = Process.GetProcessesByName(processName)[0];
        IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);

        IntPtr arAddress = process.MainModule.BaseAddress + 0x185B7A;
        IntPtr PumpAddress = process.MainModule.BaseAddress + 0x185491;

        IntPtr cameraUpDownAddress = process.MainModule.BaseAddress + 0x282469;
        IntPtr cameraLeftRightAddress = process.MainModule.BaseAddress + 0x282474;
        IntPtr cameraFunAddress = process.MainModule.BaseAddress + 0x282495;
        IntPtr sidewaysCameraAddress = process.MainModule.BaseAddress + 0x281FDA;

        Main:
        Console.WriteLine("Protoverse Memory Patcher");
        Console.WriteLine("Ammo Options:");
        Console.WriteLine("1 = Infinite Ammo ON");
        Console.WriteLine("2 = Infinite Ammo OFF");

        Console.WriteLine("\nCamera Options:");
        Console.WriteLine("3 = Disable Vertical Camera");
        Console.WriteLine("4 = Restore Vertical Camera");
        Console.WriteLine("5 = Disable Horizontal Camera");
        Console.WriteLine("6 = Restore Horizontal Camera");
        Console.WriteLine("7 = Fun Camera Mode");
        Console.WriteLine("8 = Restore Normal Camera");
        Console.WriteLine("9 = Sideways Camera");
        Console.WriteLine("10 = Restore Normal View");


        Console.WriteLine("\nOther Options:");
        Console.WriteLine("11 = Custom Projectile Speed");
        Console.WriteLine("12 = Custom Max Speed");
        Console.Write("Your choice: ");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Patch(hProcess, arAddress, arAmmo);
                Patch(hProcess, PumpAddress, PumpAmmo);
                Console.WriteLine("Infinite Ammo Enabled");
                Console.WriteLine("ENTER to go back...");
                Console.ReadLine();
                Main();
                break;
            case "2":
                Patch(hProcess, arAddress, arAmmoOG);
                Patch(hProcess, PumpAddress, PumpAmmoOG);
                Console.WriteLine("Infinite Ammo Disabled");
                Console.WriteLine("ENTER to go back...");
                Console.ReadLine();
                Main();
                break;
            case "3":
                Patch(hProcess, cameraUpDownAddress, CameraUpDownPatch);
                Console.WriteLine("Vertical Camera Movement Disabled");
                Console.WriteLine("ENTER to go back...");
                Console.ReadLine();
                Main();
                break;
            case "4":
                Patch(hProcess, cameraUpDownAddress, CameraUpDownOG);
                Console.WriteLine("Vertical Camera Movement Restored");
                Console.WriteLine("ENTER to go back...");
                Console.ReadLine();
                Main();
                break;
            case "5":
                Patch(hProcess, cameraLeftRightAddress, CameraLeftRightPatch);
                Console.WriteLine("Horizontal Camera Movement Disabled");
                Console.WriteLine("ENTER to go back...");
                Console.ReadLine();
                Main();
                break;
            case "6":
                Patch(hProcess, cameraLeftRightAddress, CameraLeftRightOG);
                Console.WriteLine("Horizontal Camera Movement Restored");
                Console.WriteLine("ENTER to go back...");
                Console.ReadLine();
                Main();
                break;
            case "7":
                Patch(hProcess, cameraFunAddress, CameraFunPatch);
                Console.WriteLine("Fun Camera Mode Enabled");
                Console.WriteLine("ENTER to go back...");
                Console.ReadLine();
                Main();
                break;
            case "8":
                Patch(hProcess, cameraFunAddress, CameraFunOG);
                Console.WriteLine("Normal Camera Restored");
                Console.WriteLine("ENTER to go back...");
                Console.ReadLine();
                Main();
                break;
            case "9":
                Patch(hProcess, sidewaysCameraAddress, SidewaysCameraPatch);
                Console.WriteLine("Sideways Camera Enabled");
                Console.WriteLine("ENTER to go back...");
                Console.ReadLine();
                Main();
                break;
            case "10":
                Patch(hProcess, sidewaysCameraAddress, SidewaysCameraOG);
                Console.WriteLine("Normal Camera View Restored");
                Console.WriteLine("ENTER to go back...");
                Console.ReadLine();
                Main();
                break;
            case "11":
                Console.Write("Enter custom value:");
                string customvaluestring = Console.ReadLine();
                float customvalue = float.Parse(customvaluestring);
                IntPtr modulebase = client.GetModuleBase("Protoverse.exe");
                client.WriteFloat(modulebase + 0x19B9990, customvalue);
                Console.WriteLine("Set custom projectile speed.");
                Console.WriteLine("ENTER to go back...");
                Console.ReadLine();
                Main();
                break;
            case "12":
                Console.Write("Enter custom value:");
                string customvaluestringspeed = Console.ReadLine();
                float customvaluespeed = float.Parse(customvaluestringspeed);
                IntPtr modulebase1 = client.GetModuleBase("Protoverse.exe");
                client.WriteFloat(modulebase1 + 0xDF1BF8, customvaluespeed);
                Console.WriteLine("Set custom max speed.");
                Console.WriteLine("ENTER to go back...");
                Console.ReadLine();
                Main();
                break;

            default:
                Console.WriteLine("Invalid input!");
                Console.WriteLine("ENTER to go back...");
                Console.ReadLine();
                Main();
                break;
        }

        CloseHandle(hProcess);
    }
    public void NiceTitle()
    {
        while (true)
        {
            Console.Title = " made by DewzaCSharp " + GenerateRandomNumber();
        }
    }
    private static string GenerateRandomNumber(int length = 35)
    { 
        Random random = new Random();
        string alphanumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    char[] serial = new char[length];
        for (int i = 0; i < length; i++)
        {
            serial[i] = alphanumeric[random.Next(alphanumeric.Length)];
        }
        return new string(serial);
    }
}