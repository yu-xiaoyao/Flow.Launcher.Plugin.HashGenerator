using System;
using System.Text;

namespace Flow.Launcher.Plugin.HashGenerator;

public class Main_Test
{
    public static void Main()
    {
        test_1();
    }

    private static void test_1()
    {
        var input = "1爱你23456543212";
        Console.WriteLine(HashHelper.CalcMd5_32(Encoding.UTF8.GetBytes(input)));
        Console.WriteLine(HashHelper.CalcMd5_16(Encoding.UTF8.GetBytes(input)));
        Console.WriteLine(HashHelper.CalcSha1(Encoding.UTF8.GetBytes(input)));
        Console.WriteLine(HashHelper.CalcSha256(Encoding.UTF8.GetBytes(input)));
        Console.WriteLine(HashHelper.CalcSha384(Encoding.UTF8.GetBytes(input)));
        Console.WriteLine(HashHelper.CalcSha512(Encoding.UTF8.GetBytes(input)));
    }
}