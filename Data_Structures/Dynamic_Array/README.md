# Dynamic Array

![Dynamic Array Visual](../../assets/images/dynamic_array_visual.svg)

## Summary :book:
A dynamic array is an array with a big improvement: automatic resizing. 
> A dynamic array expands as you add more elements. So you don't need to determine the size ahead of time. 

## Strengths :white_check_mark:
- Fast lookups
> Just like arrays, retrieving the element at a given index takes O(1) time. 
- Variable size
> You can add as many items as you want, and the dynamic array will expand to hold them. 
- Cache-friendly
> Just like arrays, dynamic arrays place items right next to each other in memory, making efficient use of caches. 

## Weaknesses :x:
- Slow worst-case appends
> Usually, adding a new element at the end of the dynamic array takes O(1) time. But if the dynamic array doesn't have any room for the new item, it'll need to expand, which takes O(n) time. 
- Costly inserts and deletes
> Just like arrays, elements are stored adjacent to each other. So adding or removing an item in the middle of the array requires "scooting over" other elements, which takes O(n) time. 

## Time Complexity :hourglass:
| Operation  | Average Case | Worst Case |
| ---------- | ------------ | ---------- |
| space      |    O(n)      |    O(n)    |
| lookup     |    O(1)      |    O(1)    |
| append     |    O(1)      |    O(n)    |
| insert     |    O(n)      |    O(n)    |
| delete     |    O(n)      |    O(n)    |
