ROS Noetic, Ubuntu 20.04LTS, Python 3.8.10

## On node start

When the node class is initialised, a second init function is called that connects, activates and calibrates the gripper. This is done with a second init function as dictated by the ```asyncio``` flow, on which the underlying gripper communications are based.

#### Asyncio peculiarities

Using ```asyncio``` means that it is necessary to use the special asyncio function structure, of which you can read here https://docs.python.org/3/library/asyncio.html.

The basics are that asycnio functions have to be defined as ```asyncio def func(foo, bar)``` and called as ```asyncio.run(func(foo, bar))``` when outside the asyncio function or as ```await func(foo, bar)``` when inside the asyncio function.

## Getting motion commands

For motion commands the node (```node.py```) subscribes to ```/gripper_command``` and expects a single string in the form ```position:speed:force```

There are two movement commands available:
1. move - starts moving to set position, returns command send success/fail
2. attempt move - attempts to move towards set position, returns last successful position and move status (stopped due to inner/outer object or at destination)

Only ```attempt move``` is currently supported

## Publishing gripper state

Gripper state is published under two publishers ```/gripper_pos``` and ```/gripper_state```. The first one will be a number between 0-255, where smaller number means more open. The second can be four different strings depending on the state: 

```
MOVING
STOPPED_OUTER_OBJECT
STOPPED_INNER_OBJECT
AT_DEST
```

##More
There are some other available commands and status requests in ```gripper.py``` not currently implemented in ```gripperControl.py```:

        minimum position - 0-255, smaller number means more open
        maximum position - 0-255, larger number means more closed
        open position - 0-255, means position considered open for gripper
        closed position - 0-255, means position considered closed for gripper
        is open - bool, is current position considered fully open
        is closed - bool, is current position considered fully closed
        current position - 0-255, open-closed
        

