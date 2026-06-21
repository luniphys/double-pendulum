# src/double-pendulum.CLI

Console application for running the double pendulum simulation in a terminal. Outputs position data in real time and optionally exports it as JSON. Also the project used for the Docker image.

## Contents

- `InputHandler.cs`: Prompts the user to enter physical parameters (lengths, masses, initial angles, damping) with range validation
- `SimulationPrinter.cs`: Runs the simulation loop, prints `(t, X1, Y1, X2, Y2)` to the console at each step, and handles JSON export to `output/`
- `Program.cs`: Entry point

## Run

```sh
dotnet run --project src/double-pendulum.CLI
```

Or via Docker (see root README for instructions on build/pull):

```sh
docker run --rm -it \
    -v "$(pwd)/output:/app/output" \
    double-pendulum
```
