from random import shuffle

number_of_unique_instances = 10
number_of_instance_files = 50


for j in range(1, number_of_instance_files + 1):
    f = open("K%r_param2.txt" % j,"w+")

    f.write("numberOfTrials:10\n")
    f.write("numberOfBlocks:1\n")
    
    f.write("numberOfInstances:10\n")

    y=[]
    feedback = []

    temp_list = list(range(1, 11))
    shuffle(temp_list)

    y = temp_list
    feedback.append(1)
        
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
    f.write("numberOfBlocks:1\n")
    
    f.write("numberOfInstances:10\n")

    y=[]
    feedback = []

    temp_list = list(range(1, 11))
    shuffle(temp_list)

    y = temp_list
    feedback.append(0)
        
    KP = "instanceRandomization:[" + ",".join(str(num) for num in y) + "]\n"
    print(KP)
    
    f.write(KP)
    
    FB = "feedback:[" + ",".join(str(num) for num in feedback) + "]\n"
    print(FB)
    f.write(FB)
    
    f.close()

