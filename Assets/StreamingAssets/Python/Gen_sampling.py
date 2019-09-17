from random import shuffle
import random

number_of_unique_instances = 80

for j in range(1, number_of_unique_instances + 1):
    f = open("s%r.txt" % j,"w+")
    left = random.randint(1,30)

    right = random.randint(1,30)
    while left == right:
        right = random.randint(1,30)
        
    f.write(f"LeftBox:{left}\n")
    f.write(f"RightBox:{right}\n")
    

    if left>right:
        f.write("solution:0\n")
    elif right>left:
        f.write("solution:1\n")
    f.close()
