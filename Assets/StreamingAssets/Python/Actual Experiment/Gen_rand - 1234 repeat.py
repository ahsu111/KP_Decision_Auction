from random import shuffle

number_of_unique_instances = 40
number_of_instance_files = 50


trial_per_block = 15

for j in range(1, number_of_instance_files + 1):
    rand1 = list(range(1,121,4))
    rand2 = list(range(2,121,4))
    rand3 = list(range(3,121,4))
    rand4 = list(range(4,121,4))
    
    shuffle(rand1)
    shuffle(rand2)
    shuffle(rand3)
    shuffle(rand4)
    
    f = open("K%r_param2.txt" % j,"w+")

    f.write(f"numberOfTrials:{trial_per_block}\n")
    f.write("numberOfBlocks:8\n")
    
    f.write(f"numberOfInstances:{trial_per_block*8}\n")

    temp_list=[]
    feedback = []

    for rand in [rand1, rand2, rand3, rand4]:
        #print(rand, rand[:15], rand[15:])
        temp_list.append([rand[:15], 0])
        temp_list.append([rand[15:], 1])


    shuffle(temp_list)

    y=[]
    feedback=[]
    for i in temp_list:
        y = y + i[0]
        feedback.append(i[1])

        
    KP = "instanceRandomization:[" + ",".join(str(num) for num in y) + "]\n"
    print(KP)
    
    f.write(KP)
    
    FB = "feedback:[" + ",".join(str(num) for num in feedback) + "]\n"
    print(FB)
    f.write(FB)
    
    
    f.close()


for j in range(1, number_of_instance_files + 1):
    f = open("S%r_param2.txt" % j,"w+")

    f.write(f"numberOfTrials:{trial_per_block}\n")
    f.write("numberOfBlocks:8\n")
    
    f.write(f"numberOfInstances:{trial_per_block*2}\n")


    more_list = [0,0,1,1]

    more_list2 = more_list.copy()

    total_list = [[x, 0] for x in more_list] + [[z, 1] for z in more_list2]
    shuffle(total_list)
    print(total_list)
    y=[]
    feedback = []
    for i in total_list:
        temp_list = list(range(1, trial_per_block+1))
        shuffle(temp_list)

        y = y + [i[0]*trial_per_block + j for j in temp_list]
        feedback.append(i[1])
        
    KP = "instanceRandomization:[" + ",".join(str(num) for num in y) + "]\n"
    print(KP)
    
    f.write(KP)
    
    FB = "feedback:[" + ",".join(str(num) for num in feedback) + "]\n"
    print(FB)
    f.write(FB)
    
    
    f.close()


