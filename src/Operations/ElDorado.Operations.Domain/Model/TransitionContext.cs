using System;

namespace ElDorado.Operations.Domain.Model;

public class TransitionContext
{
    public State? From { get; set; }
    public State? To { get; set; }
}
