# Linked List

![Linked List Visual](../../assets/images/linked_list_visual.svg)

## Summary :book:
A linked list organizes items sequentially, with each item storing a pointer to the next one. 
> Picture a linked list like a chain of paperclips linked together. It's quick to add another paperclip to the top or bottom. It's even quick to insert one in the middle—just disconnect the chain at the middle link, add the new paperclip, then reconnect the other half. 
> 
> An item in a linked list is called a node. The first node is called the head. The last node is called the tail. 

![Linked List Visual 2](../../assets/images/linked_list_visual_2.svg)

> Unlike an array, consecutive items in a linked list are not necessarily next to each other in memory. 

## Strengths :white_check_mark:
- Fast operations on the ends
> Adding elements at either end of a linked list is O(1). Removing the first element is also O(1). 
- Flexible Size
> There's no need to specify how many elements you're going to store ahead of time. You can keep adding elements as long as there's enough space on the machine. 

## Weaknesses :x:
- Costly lookups
> To access or edit an item in a linked list, you have to take O(i) time to walk from the head of the list to the ith item. 

## Time Complexity :hourglass:
| Operation  | Worst Case |
| ---------- | ---------- |
| space      |    O(n)    |
| prepend    |    O(1)    |
| append     |    O(1)    |
| lookup     |    O(n)    |
| insert     |    O(n)    |
| delete     |    O(n)    |

## Doubly Linked Lists
> In a basic linked list, each item stores a single pointer to the next element.

In a doubly linked list, items have pointers to the next and the previous nodes. 

![Linked List Visual 3](../../assets/images/linked_list_visual_3.svg)

Doubly linked lists allow us to traverse our list backwards.
> In a singly linked list, if you just had a pointer to a node in the middle of a list, there would be no way to know what nodes came before it.

Not a problem in a doubly linked list. 
