using System;

[Serializable]
public struct Pair<T, U> {

    public Pair(T a, U b) {
        this.a = a;
        this.b = b;
    }

    public T a;
    public U b;

}
