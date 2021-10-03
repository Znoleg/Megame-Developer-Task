using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Range<T>
{
    public T Min;
    public T Max;

    public Range(T min, T max)
    {
        Min = min;
        Max = max;
    }
}

