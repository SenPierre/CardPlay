using System.Collections.Generic;
using Godot;

public partial class ParticleHandler : Node
{
    private List<CpuParticles2D> m_ParticleEmitters = new List<CpuParticles2D>();
    private int m_bStopEmitNextFrame = 2;
	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public override void _Ready()
    {
        ProcessPriority = 500;
        foreach(Node node in GetChildren())
        {
            CpuParticles2D foundParticleEmitter = (CpuParticles2D)node;
            if (foundParticleEmitter != null)
            {
                m_ParticleEmitters.Add(foundParticleEmitter);
            }
        }
    }

	// -----------------------------------------------------------------
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// -----------------------------------------------------------------
	public override void _Process(double delta)
	{
        if (m_bStopEmitNextFrame == 1)
        {
            foreach(CpuParticles2D emitter in m_ParticleEmitters)
            {
                emitter.Emitting = false;
            }
            m_bStopEmitNextFrame = 0;
        }
        else if (m_bStopEmitNextFrame > 1)
        {
            m_bStopEmitNextFrame--;
        }
	}

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public void EmitBurst()
    {
        foreach(CpuParticles2D emitter in m_ParticleEmitters)
        {
            emitter.Emitting = true;
        }
        m_bStopEmitNextFrame = 2;
    }

}
