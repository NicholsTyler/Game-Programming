# Array

![Array Visual](../../assets/images/array_preview.svg)

## Summary :book:
An array organizes items sequentially, one after another in memory.
> Each position in the array has an **index**, starting at 0.

## Strengths :white_check_mark:
- Fast lookups
> Retrieving the element at a given index takes O(1) time, regardless of the length of the array.
- Fast appends
> Adding a new element at the end of the array takes O(1) time, if the array has space.

## Weaknesses :x:
- Fixed size
> You need to specify how many elements you're going to store in your array ahead of time. (Use a [Dynamic Array](https://github.com/NicholsTyler/Game-Programming/Data_Structures/Dynamic_Array) if this is an issue)
- Costly inserts and deletes
> You have to "scoot over" the other elements to fill in or close gaps, which takes worst-case O(n) time. 

## Time Complexity :hourglass:
### Worst Case
- **space** = O(n)
- **lookup** = O(1)
- **append** = O(1)
- **insert** = O(n)
- **delete** = O(n)
