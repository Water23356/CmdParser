using CmdParser.Sample;

internal class Program
{
    private static void Main(string[] args)
    {
        RrunSample(7, args);
    }

    private static void RrunSample(int index, string[] args)
    {
        switch (index)
        {
            case 1:
                new Sample1().RunSample();
                break;

            case 2:
                new Sample2().RunSample();
                break;

            case 3:
                new Sample3().RunSample();
                break;

            case 4:
                new Sample4().RunSample(args);
                break;

            case 5:
                new Sample5().RunSample();
                break;

            case 6:
                new Sample6().RunSample();
                break;

            case 7:
                new Sample7().RunSample();
                break;

            default:
                new Sample1().RunSample();
                break;
        }
    }
}