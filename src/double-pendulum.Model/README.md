# src/double-pendulum.Model

Class library containing the physics engine. Used by both `double-pendulum.Presentation` and `double-pendulum.CLI`.

## Contents

- `PendulumParameters.cs`: Holds physical initial-state parameters passed in at construction (lengths, masses, initial angles in degrees, damping coefficient)
- `PendulumPhysics.cs`: Numerical integrator that maintains the state vector $(\theta_1, \theta_2, \omega_1, \omega_2)$
