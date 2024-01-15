# UnityInteractor

![Unity Version](https://img.shields.io/badge/Unity-2021.1%2B-blue.svg)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

![Preview](using.gif)

## Description

UnityInteractor Asset is a convenient tool for creating and managing iterative systems in Unity using coroutines.

## Key Features

1. **Node Structure:**

   - The system provides a graphical interface for creating and editing a node structure representing the iterative system.

2. **Coroutines in Nodes:**

   - Each node in the system represents a coroutine that can run independently of other nodes.

3. **Tree-like Representation:**

   - The tree of nodes forms a tree-like structure, easily readable and editable in the Unity editor.

4. **Parallel Actions:**
   - Ability to create parallel nodes representing concurrent actions for objects.

## Integration

1. **Simple Integration:**

   - Easy integration with Unity objects using a component that can be attached to your game object.

2. **API for Interaction:**
   - Provides an API for interacting with the system, allowing you to start, stop, and modify coroutine states in real-time.

## Sample

1. Attach the InteractionObject component.
2. To edit the behavior, click on the "Open Sequence Editor" button
3. Create the behavior of an object using a graphical representation.
4. To start a sequence, you need to call the StartSequence() method on an InteractionObject object instance or click on the StartTestSequence button in Runtime mode.

Ability to add your own nodes by inheriting from the BaseInteractionAction base class.
To serialize your fields and properties into nodes, you need to use the [SerializeFieldNode] attribute
