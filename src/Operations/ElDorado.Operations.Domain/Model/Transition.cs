using System;

namespace ElDorado.Operations.Domain.Model;

public class Transition
{
    public State? From { get; set; }
    public State? To { get; set; }
    public object TransitionContext { get; set; }
}
