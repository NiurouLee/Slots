// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/19/19:03
// Ver : 1.0.0
// Description : IExternelEventScheduler.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public interface IExternalEventScheduler
    {
        void AttachToListener();
        void DetachFromListener();
    }
}