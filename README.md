# CircleArena

===================================

Develop a WPF (C#) application that is a primitive graphics program. This program’s purpose is to create and manipulate a set of circular shapes shown on a canvas. Your program must:
•	Provide a button that creates a new circle on the canvas at a random visible location,
•	Allow the user to “drag” one of the circles to a new location on the canvas, and
•	Provide buttons that perform undo and redo operations.

The undo/redo system must adhere to the following requirements:
•	Undoing creating a circle will delete that circle. Redoing it will make that appear at the point where it was initially created.
•	Undoing moving a circle will return it to the position where it was at the beginning of the mouse operation. Redoing it will make it appear at the location where it was at the end of the mouse operation.
•	The undo (and redo) buttons should be disabled if there are no undo (or redo) operations that can be performed.

Your application should:
•	Utilise an MVVM-based design,
•	Have code written as professionally as possible (show us your best work!),
•	Be clear and understandable to someone unfamiliar with the code, and
•	Be thoroughly tested and should not exhibit any crash bugs.
