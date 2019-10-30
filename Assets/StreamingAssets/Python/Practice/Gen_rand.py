from random import shuffle

number_of_instance_files = 50

trial_per_block = 15

for j in range(1, number_of_instance_files + 1):
    f = open("K%r_param2.txt" % j,"w+")

    f.write(f"numberOfTrials:{trial_per_block}\n")
    f.write("numberOfBlocks:2\n")
    
    f.write(f"numberOfInstances:{trial_per_block*2}\n")


    more_list = [0,1]

    shuffle(more_list)

    total_list = [[more_list[0], 0], [more_list[1], 1]]

    shuffle(total_list)
    print(total_list)
    y=[]
    feedback = []
    for i in total_list:
        temp_list = list(range(1, trial_per_block + 1))
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


for j in range(1, number_of_instance_files + 1):
    f = open("S%r_param2.txt" % j,"w+")

    f.write(f"numberOfTrials:{trial_per_block}\n")
    f.write("numberOfBlocks:2\n")
    
    f.write(f"numberOfInstances:{trial_per_block*2}\n")


    more_list = [0,1]

    shuffle(more_list)

    total_list = [[more_list[0], 0], [more_list[1], 1]]
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


