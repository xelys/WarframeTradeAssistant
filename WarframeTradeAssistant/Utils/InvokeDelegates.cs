namespace WarframeTradeAssistant.Utils
{
    public delegate void ActionDelegate();

    public delegate void ActionDelegate<in T>(T arg);

    public delegate void ActionDelegate<in T1, in T2>(T1 arg1, T2 arg2);

    public delegate void ActionDelegate<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);

    public delegate void ActionDelegate<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    public delegate R FuncDelegate<out R>();

    public delegate R FuncDelegate<in T, out R>(T arg);

    public delegate R FuncDelegate<in T1, in T2, out R>(T1 arg1, T2 arg2);

    public delegate R FuncDelegate<in T1, in T2, in T3, out R>(T1 arg1, T2 arg2, T3 arg3);

    public delegate R FuncDelegate<in T1, in T2, in T3, in T4, out R>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
}
