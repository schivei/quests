using Microsoft.AspNetCore.Mvc;
using System;

namespace Quests.App
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BindUidAttribute : BindPropertyAttribute
    {
        public BindUidAttribute()
        {
            BinderType = typeof(UidBinder);
        }
    }
}
