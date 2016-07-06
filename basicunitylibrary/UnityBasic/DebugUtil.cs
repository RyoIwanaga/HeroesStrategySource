using UnityEngine;
using System.Diagnostics;
using System.Text;
using System.Collections;

public static class DebugUtil
{
    /// <summary>
    /// Create class : method name
    /// </summary>
    public static string FN
    {
        get
        {
            var builder = new StringBuilder();
            var method = new StackTrace().GetFrame(1).GetMethod();

            builder.Append(method.ReflectedType.Name);
            builder.Append(" : ");
            builder.Append(new StackTrace().GetFrame(1).GetMethod().ToString());
            builder.Append(": ");

            return builder.ToString();
        }
    }
}
