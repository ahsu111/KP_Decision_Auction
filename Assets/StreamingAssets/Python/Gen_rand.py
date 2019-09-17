from random import shuffle

number_of_unique_instances = 80
number_of_instance_files = 50


x = [i for i in range(1,number_of_unique_instances + 1)]
print(x)

for j in range(1, number_of_instance_files + 1):
    f = open("K%r_param2.txt" % j,"w+")

    f.write("numberOfTrials:10\n")
    f.write("numberOfBlocks:8\n")
    
    f.write("numberOfInstances:80\n")

    shuffle(x)
    KP = "instanceRandomization:[" + ",".join(str(num) for num in x) + "]\n"
    print(KP)
    f.write(KP)

    f.close()

    f = open("S%r_param2.txt" % j,"w+")

    f.write("numberOfTrials:10\n")
    f.write("numberOfBlocks:8\n")
    
    f.write("numberOfInstances:80\n")

    shuffle(x)
    KP = "instanceRandomization:[" + ",".join(str(num) for num in x) + "]\n"
    print(KP)
    f.write(KP)

    f.close()
