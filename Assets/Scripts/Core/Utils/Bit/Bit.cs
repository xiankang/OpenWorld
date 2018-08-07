using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bit{
    public static uint BIT(int x)
    {
        return 1u << x;
    }

    public static UInt64 BIT64(int x)
    {
        return 1ul << x;
    }
}
