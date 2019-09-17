for i in range(1,17):
    print(i)
    with open(f"i{i}.txt") as f:
        lines = f.readlines()
        lines = [l for l in lines]
        for j in range(i,81,16): #16+i
            print("j is equal to", j)
            with open(f"k{j}.txt", "w") as f1:
                f1.writelines(lines)
            f1.close()
    f.close()
