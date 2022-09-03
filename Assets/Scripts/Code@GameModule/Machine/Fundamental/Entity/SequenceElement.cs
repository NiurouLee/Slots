// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/05/12:58
// Ver : 1.0.0
// Description : SequenceElement.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class SequenceElement
    {
        public readonly ElementConfig config;
        public object elementCustomData;
        public readonly MachineContext machineContext;

        public SequenceElement(ElementConfig inConfig, MachineContext inMachineContext, object inElementCustomData = null)
        {
            config = inConfig;
            machineContext = inMachineContext;
            elementCustomData = inElementCustomData;
        }
    }
}