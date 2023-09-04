que = {
    "A":{
        "ja":["0"],
        "nein":["8"]
        }
}
ans = [("ja","1"),("vielleicht","3"),("nein","8")]

def QuestionHasAnswer(q,a):
    for i in range(0,len(q)):
        (x,y) = q[i]
        if(x == a):
            return True, i
    return False, -1

q = "A"

for (a,i) in ans:
    contains, index = QuestionHasAnswer(que[q],a)
    if contains:
        if i not in que[q][index][1]:
            que[q][index][1] += [i]
    else:
        que[q] += [(a,[i])]

print(que[q])

{
    'A': 
    #que[q]
    [('ja', [0]), ('nein', [8]), ('ja', [1]), ('vielleicht', [3]), ('nein', [8])]
}

my_list = [(1, 2), (3, 4), (5, 6)]

if any(x==3 for (x,y) in my_list):
    print("Der Wert 3 ist in der Liste enthalten.")