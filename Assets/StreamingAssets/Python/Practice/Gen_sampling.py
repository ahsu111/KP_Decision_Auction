from random import shuffle
import random

number_of_unique_instances = 30

base = 313

more_list = [24, 58] #35, 46, 

#more_list2 = more_list.copy()

shuffle(more_list)

#shuffle(more_list2)

total_list = [i + base for i in more_list] #+ [j + base for j in more_list2] 

print(total_list)

for j in range(1, number_of_unique_instances + 1):
    dots_left_right = [313, total_list[(j-1)//15]]

    shuffle(dots_left_right)
    
    f = open("s%r.txt" % j,"w+")
    left = dots_left_right[0]

    right = dots_left_right[1]
    
##    while left == right:
##        right = random.randint(1,30)
        
    f.write(f"LeftBox:{left}\n")
    f.write(f"RightBox:{right}\n")
    

    if left>right:
        f.write("solution:0\n")
    elif right>left:
        f.write("solution:1\n")
    f.close()
