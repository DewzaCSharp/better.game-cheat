using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

public class Dewz
{
	private Process proc;

	[DllImport("Kernel32.dll")]
	private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);

	[DllImport("kernel32.dll")]
	private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int size, IntPtr lpNumberOfBytesWritten);

	[DllImport("kernel32.dll")]
	private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

	[DllImport("kernel32.dll")]
	private static extern bool CloseHandle(IntPtr hObject);

	public Dewz(string procName)
	{
		proc = SetProcess(procName);
	}

	public Process GetProcess()
	{
		return proc;
	}

	public Process SetProcess(string procName)
	{
		proc = Process.GetProcessesByName(procName)[0];
		if (proc == null)
		{
			throw new InvalidOperationException("Process was not found, are you using the correct bit version and have no miss-spellings?");
		}
		return proc;
	}

	public IntPtr GetModuleBase(string moduleName)
	{
		if (string.IsNullOrEmpty(moduleName))
		{
			throw new InvalidOperationException("moduleName was either null or empty.");
		}
		if (proc == null)
		{
			throw new InvalidOperationException("process is invalid, check your init.");
		}
		try
		{
			if (moduleName.Contains(".exe") && proc.MainModule != null)
			{
				return proc.MainModule.BaseAddress;
			}
			foreach (ProcessModule module in proc.Modules)
			{
				if (module.ModuleName == moduleName)
				{
					return module.BaseAddress;
				}
			}
		}
		catch (Exception)
		{
			throw new InvalidOperationException("Failed to find the specified module, search string might have miss-spellings.");
		}
		return IntPtr.Zero;
	}
	
	public bool WriteBytes(IntPtr address, byte[] newbytes)
	{
		return WriteProcessMemory(proc.Handle, address, newbytes, newbytes.Length, IntPtr.Zero);
	}

	public bool WriteFloat(IntPtr address, float value)
	{
		return WriteBytes(address, BitConverter.GetBytes(value));
	}

	public bool Nop(IntPtr address, int length)
	{
		byte[] array = new byte[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = 144;
		}
		return WriteBytes(address, array);
	}

	public bool Nop(IntPtr address, int offset, int length)
	{
		byte[] array = new byte[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = 144;
		}
		return WriteBytes(address + offset, array);
	}
}
