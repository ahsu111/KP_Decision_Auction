for i in range(1,17):
    with open(f"i{i}.txt") as f:
        lines = f.readlines()
        lines = [l for l in lines]
        for j in range(16+i,81,16):
            with open(f"i{j}.txt", "w") as f1:
                f1.writelines(lines)
