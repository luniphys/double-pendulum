# tests

This directory contains xUnit unit tests for the pendulum physics simulation.

## Contents

- `PendulumPhysicsTests.cs`: Unit tests for `PendulumPhysics.cs`

## Notes

- Tests cover one explicit case for each method: initialization, derivative calculation, RK4 integration, simulation step, and coordinate transformation.
- Physical property tests: pendulum at rest stays at rest, Cartesian position at various angles, and energy conservation with and without damping.
