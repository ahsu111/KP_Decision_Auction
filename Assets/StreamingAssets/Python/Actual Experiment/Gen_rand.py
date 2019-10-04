from random import shuffle

number_of_unique_instances = 40
number_of_instance_files = 50


for j in range(1, number_of_instance_files + 1):
    f = open("K%r_param2.txt" % j,"w+")

    f.write("numberOfTrials:10\n")
    f.write("numberOfBlocks:8\n")
    
    f.write("numberOfInstances:80\n")



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
        temp_list = list(range(1, 11))
        shuffle(temp_list)

        y = y + [i[0]*10 + j for j in temp_list]
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

    f.write("numberOfTrials:10\n")
    f.write("numberOfBlocks:8\n")
    
    f.write("numberOfInstances:40\n")


    more_list = list(range(4))

    more_list2 = more_list.copy()

    shuffle(more_list)

    shuffle(more_list2)

    total_list = [[x, 0] for x in more_list] + [[z, 1] for z in more_list2]
    shuffle(total_list)
    print(total_list)
    y=[]
    feedback = []
    for i in total_list:
        temp_list = list(range(1, 11))
        shuffle(temp_list)

        y = y + [i[0]*10 + j for j in temp_list]
        feedback.append(i[1])
        
    KP = "instanceRandomization:[" + ",".join(str(num) for num in y) + "]\n"
    print(KP)
    
    f.write(KP)
    
    FB = "feedback:[" + ",".join(str(num) for num in feedback) + "]\n"
    print(FB)
    f.write(FB)
    
    
    f.close()


