from random import shuffle
import random

number_of_unique_instances = 10

base = 300

total_list = 330

for j in range(1, number_of_unique_instances + 1):
    dots_left_right = [300, 330]

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
