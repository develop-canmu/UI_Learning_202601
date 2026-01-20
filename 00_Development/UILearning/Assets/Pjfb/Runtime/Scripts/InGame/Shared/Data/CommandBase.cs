using System;
using System.Collections;
using System.Collections.Generic;

namespace Pjfb.InGame
{
    public abstract class CommandBase
    {
        public bool IsPreProcessed { get; protected set; }

        public CommandBase() { }

        public abstract float Delay { get; }

        public abstract bool CanProcess();

        public abstract bool IsTooLateToProcess();

        public abstract void PreProcessCommand(float elapsedSec);

        public abstract bool ProcessCommand(float elapsedSec);

        public abstract void ProcessCommandOnlyResult();
    }
}