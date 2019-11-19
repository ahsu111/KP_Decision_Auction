from random import shuffle

number_of_unique_instances = 40
number_of_instance_files = 50


trial_per_block = 15

for j in range(1, number_of_instance_files + 1):
    temp_rand1 = list(range(1,121,4))
    temp_rand2 = list(range(2,121,4))
    temp_rand3 = list(range(3,121,4))
    temp_rand4 = list(range(4,121,4))
    
    shuffle(temp_rand1)
    shuffle(temp_rand2)
    shuffle(temp_rand3)
    shuffle(temp_rand4)

    rand1 = temp_rand1 + temp_rand4
    rand2 = temp_rand2 + temp_rand3
    
    shuffle(rand1)

    shuffle(rand2)
    
    f = open("K%r_param2.txt" % j,"w+")

    f.write(f"numberOfTrials:{trial_per_block}\n")
    f.write("numberOfBlocks:8\n")
    
    f.write(f"numberOfInstances:{trial_per_block*8}\n")

    more_list = list(range(0,8,2))

    more_list2 = list(range(1,8,2))

    shuffle(more_list)

    shuffle(more_list2)

    total_list = [[x, 0] for x in more_list] + [[z, 1] for z in more_list2]
    shuffle(total_list)
    print(total_list)
    y=[]
    feedback = []
    for i in total_list:
        if (i[0]<4):
            temp_list = list(range(trial_per_block))
            shuffle(temp_list)
            y = y + [rand1[i[0]*trial_per_block + j] for j in temp_list]
            feedback.append(i[1])
        else:
            temp_list = list(range(trial_per_block))
            shuffle(temp_list)

            y = y + [rand2[(i[0]-4)*trial_per_block + j] for j in temp_list]
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


