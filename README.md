# Network Service

A WPF desktop application for monitoring and managing network entities.

The application provides a visual interface for managing monitored entities, arranging them in a network layout, creating connections between them, filtering and managing entity data, and visualizing recent measurements. It also communicates with an external monitoring application through a TCP connection.

## Features

### Network Entities

The **Network Entities** view provides management of all entities currently being monitored.

* Add new entities
* Delete existing entities
* Validate entity data before insertion
* Prevent duplicate entity IDs
* Display entities grouped by type
* Filter entities by:

  * Entity type
  * ID
  * ID comparison (`<`, `=`, `>`)
  * Measurement value range
* Reset active filters
* Success and error notifications

Entities currently use two supported types:

* **RTD**
* **TC**

Each entity contains an ID, name, type, current measurement value, timestamp, and a history of the five most recent measurements.

---

### Network Display

The **Network Display** provides a visual representation of the monitored network.

Entities can be dragged from the entity list into predefined network slots.

Supported operations include:

* Drag entities into the network
* Move entities between slots
* Remove entities from the network
* Connect two entities
* Prevent duplicate connections
* Automatically rewire connections when entities are moved
* Disconnect entities when they are removed
* Undo the latest operation
* Undo all operations
* Redo previously undone operations

The display consists of a grid of predefined slots where monitored entities can be positioned.

---

### Measurement Graphs

The **Measurement Graphs** view displays the recent measurements of an entity.

For each entity, the application keeps the five latest measurement values and their timestamps.

The graph:

* Displays recent measurement values
* Connects measurements into a graph
* Displays measurement timestamps
* Updates when an entity's value changes
* Allows navigation between entities using **Next** and **Previous**
* Visually distinguishes measurement points and connections

The graph is implemented using `GraphPoint` and `GraphEdge` models and is updated through property-change notifications.

---

## TCP Communication

The application contains a TCP listener used to communicate with an external monitoring/simulator application.

The listener runs on:

```text
Port: 25675
Address: 0.0.0.0
Protocol: TCP
```

When the application starts, it creates a `TcpListener` and waits for incoming connections. Each connection is processed using the thread pool.

### Request: Object Count

The external application can request the number of monitored objects by sending:

```text
Need object count
```

The Network Service responds with the current number of entities:

```text
4
```

### Request: Entity Measurement Update

The external application can send an entity measurement update in the following format:

```text
Entity_<ID>:<Value>
```

For example:

```text
Entity_1:272
```

The application parses the message, finds the corresponding entity, updates its measurement and timestamp, and appends the update to the log file.

The entity's latest five measurements are retained and used by the measurement graph.

---

## Architecture

The application follows the **MVVM (Model-View-ViewModel)** pattern.

```text
NetworkService/
│
├── Model/
│   ├── Entity.cs
│   ├── EntityByType.cs
│   └── EntityType.cs
│
├── ViewModel/
│   ├── MainWindowViewModel.cs
│   ├── NetworkEntitiesViewModel.cs
│   ├── NetworkDisplayViewModel.cs
│   └── MeasurementGraphViewModel.cs
│
├── Views/
│   ├── NetworkEntitiesView.xaml
│   ├── NetworkDisplayView.xaml
│   └── MeasurementGraphView.xaml
│
├── Helpers/
│   ├── Commands/
│   ├── Converters/
│   ├── Display/
│   ├── Filters/
│   ├── Graph/
│   └── Validation/
│
├── Data/
│   ├── Images/
│   │   ├── RTD.png
│   │   └── TC.png
│   └── log.txt
│
├── App.xaml
├── MainWindow.xaml
├── App.config
└── NetworkService.csproj
```

### Model

The model layer contains the application's core data structures.

`Entity` represents a monitored network entity and contains:

* `ID`
* `Name`
* `Type`
* `Value`
* `TimeStamp`
* Last five values
* Last five timestamps

It also contains validation logic for required fields and invalid IDs.

`EntityType` represents an entity category and its associated image.

### ViewModel

The ViewModel layer contains the application logic exposed to the WPF views.

#### `MainWindowViewModel`

Responsible for:

* Application initialization
* Navigation between views
* Initial entity data
* TCP communication
* Entity collection management
* Global command histories
* Notifications

It also owns the shared collections used by the different views.

#### `NetworkEntitiesViewModel`

Responsible for:

* Adding entities
* Removing entities
* Validation
* Filtering
* Entity history
* Undo/redo operations

#### `NetworkDisplayViewModel`

Responsible for:

* Entity positioning
* Drag-and-drop operations
* Network connections
* Connection rewiring
* Removing entities from the display
* Undo/redo operations

#### `MeasurementGraphViewModel`

Responsible for:

* Selecting the current entity
* Loading measurement history
* Creating graph points
* Creating graph edges
* Navigating between entities
* Updating the graph when measurements change

---

## Application Navigation

The application has three primary views:

```text
┌─────────────────────────────────────────────┐
│                 NETWORK SERVICE              │
│                                             │
│  [Entities]   [Network Display]   [Graphs] │
├─────────────────────────────────────────────┤
│                                             │
│              Current View                   │
│                                             │
└─────────────────────────────────────────────┘
```

Keyboard shortcuts are available for navigation:

| Key   | View               |
| ----- | ------------------ |
| `F1`  | Network Entities   |
| `F2`  | Network Display    |
| `F3`  | Measurement Graphs |
| `Esc` | Close application  |

These shortcuts are defined as WPF key bindings in the main window.

---

## Undo / Redo

The application implements command-based history using a custom `CommandStack`.

Separate histories are maintained for:

* Entity operations
* Network display operations

Supported operations include:

```text
Undo
Undo All
Redo
```

This allows operations such as adding/removing entities, filtering, moving entities, and creating connections to be reversed.

---

## Filtering

Entities can be filtered using several criteria.

### Entity Type

Filter by the entity's type:

```text
RTD
TC
```

### ID

Filter IDs using:

```text
ID < value
ID = value
ID > value
```

### Measurement Value

Measurements can be filtered using predefined bounds.

```text
Inside bounds:
250 < value < 350

Out of bounds:
value <= 250 OR value >= 350
```

The filter implementation combines multiple criteria when they are selected.

---

## Logging

Measurement updates are written to:

```text
Data/log.txt
```

Each logged measurement follows the format:

```text
dd/MM/yyyy HH:mm:ss [TYPE] EntityName Got value: VALUE
```

For example:

```text
10/06/2026 19:44:18 [RTD] RTD-001 Got value: 442
```

The log is appended whenever an entity receives a new measurement through the TCP communication layer.

---

## Technologies

The project is built with:

* **C#**
* **WPF**
* **.NET Framework 4.7.2**
* **MVVM**
* **TCP sockets**
* **XAML**
* **MVVMLight.Messaging**
* **Microsoft.Xaml.Behaviors**
* **FontAwesome5**
* **Notification.Wpf**

The project targets `.NET Framework 4.7.2` and uses a traditional `.csproj` + `packages.config` setup.

---

## Getting Started

### Prerequisites

You will need:

* Windows
* Visual Studio 2019 or newer
* .NET Framework 4.7.2 Developer Pack
* NuGet package support

Because this is a WPF application targeting .NET Framework 4.7.2, it is intended to run on Windows.

### 1. Clone the repository

```bash
git clone https://github.com/nikolasovanj/IUIS-PZ2.git
cd IUIS-PZ2
```

### 2. Open the solution

Open:

```text
NetworkService.sln
```

in Visual Studio.

### 3. Restore NuGet packages

The project uses the following NuGet dependencies:

* `FontAwesome5`
* `Microsoft.Xaml.Behaviors.Wpf`
* `MVVMLight.Messaging`
* `Notification.Wpf`
* `System.ValueTuple`
* `System.Windows.Interactivity.WPF`

These are specified in `packages.config` and should be restored by Visual Studio/NuGet.

If packages are not restored automatically:

```text
Right click Solution
    → Restore NuGet Packages
```

### 4. Build

Build the solution using:

```text
Build → Build Solution
```

or:

```text
Ctrl + Shift + B
```

### 5. Run

Start the application with:

```text
F5
```

or:

```text
Ctrl + F5
```

The application starts with the **Network Entities** view.

---

## Initial Data

The application contains sample data so it can be used immediately after startup.

The initial entities include:

| ID | Name    | Type |
| -: | ------- | ---- |
|  1 | RTD-001 | RTD  |
|  3 | RTD-002 | RTD  |
|  4 | TSP-002 | TC   |
|  2 | TSP-001 | TC   |

The application also initializes a 4 × 4 network display containing 16 available slots.

---

## Connecting an External Simulator

The application itself acts as the TCP listener.

An external simulator should connect to:

```text
127.0.0.1:25675
```

For example:

```text
TCP Client
    │
    │ "Need object count"
    ▼
Network Service
    │
    │ "4"
    ▼
TCP Client
```

For measurement updates:

```text
TCP Client
    │
    │ "Entity_1:272"
    ▼
Network Service
    │
    ├── Update Entity #1
    ├── Update timestamp
    ├── Update measurement history
    ├── Update graph
    └── Write to log
```

The external simulator is not part of this repository, so running the WPF application by itself is sufficient for using the GUI, but live TCP updates require a compatible TCP client/simulator.

---

## Project Design

The application separates responsibilities between the different layers:

```text
                    ┌──────────────────┐
                    │     MainWindow    │
                    │       XAML        │
                    └────────┬─────────┘
                             │
                             ▼
                    ┌──────────────────┐
                    │   ViewModels     │
                    ├──────────────────┤
                    │ Entities         │
                    │ Network Display  │
                    │ Measurement Graph│
                    └────────┬─────────┘
                             │
              ┌──────────────┼──────────────┐
              ▼              ▼              ▼
          ┌────────┐    ┌──────────┐   ┌──────────┐
          │ Models │    │ Helpers  │   │ Commands │
          └────────┘    └──────────┘   └──────────┘
                             ▲
                             │
                    ┌────────┴─────────┐
                    │   TCP Listener   │
                    │    Port 25675    │
                    └──────────────────┘
```

The WPF views are kept primarily responsible for presentation, while application behavior is handled by ViewModels and supporting classes.

---

## Data Flow

A typical measurement update follows this path:

```text
External Simulator
        │
        │ Entity_1:272
        ▼
   TCP Listener
        │
        ▼
 Parse incoming message
        │
        ▼
 Find Entity #1
        │
        ├──────────────► Update Value
        │
        ├──────────────► Update Timestamp
        │
        ├──────────────► Store latest 5 readings
        │
        ├──────────────► Update Measurement Graph
        │
        └──────────────► Append to log.txt
```

Because entity properties implement change notification, the UI can react to measurement changes without the ViewModels needing to directly manipulate the visual controls.

---

## Notes

* The TCP listener uses port `25675`; make sure another application is not already using this port.
* The application expects incoming measurement messages to follow the `Entity_<ID>:<Value>` format.
* The measurement graph stores the five most recent readings for each entity.
* Entity IDs must be unique.
* The project currently contains sample entities and measurements for demonstration.
* `Data/log.txt` is used as the measurement log.

## License

No license is currently specified in the repository.
