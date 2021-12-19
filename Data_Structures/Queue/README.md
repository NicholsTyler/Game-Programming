# Queue

![Queue Visual](../../assets/images/queue_visual.gif)

## Summary :book:
A queue stores items in a first-in, first-out (FIFO) order. 
> Picture a queue like the line outside a busy restaurant. First come, first served. 

## Uses :scroll:
- Breadth-first search (BFS) uses a queue to keep track of the nodes to visit next.

## Strengths :white_check_mark:
- Fast operations
> All queue operations take O(1) time. 

## Weaknesses :x:
- VERY costly to search
> Don't use a queue if you need to operate on middle elements consistently

## Time Complexity :hourglass:
| Operation  | Worst Case |
| ---------- | ---------- |
| space      |    O(n)    |
| enqueue    |    O(1)    |
| dequeue    |    O(1)    |
| peek       |    O(1)    |
