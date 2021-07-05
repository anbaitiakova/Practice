#include <iostream>
#include "list"
class GaluaField
{
    unsigned char poly;
    unsigned int px;
public:
    GaluaField(){}

    uint8_t Add(uint8_t left, uint8_t right)
    {
        poly = left ^ right;
        return poly;
    }

    uint8_t Multiply(uint8_t left, uint8_t right, uint16_t modulo)
    {
        poly = 0;
        while (left && right)
        {
            if (right & 1)
                poly ^= left;
            if (left & 0x80)
                left = (left << 1) ^ modulo;
            else
                left <<= 1;
            right >>= 1;
        }
        return poly;
    }

    uint64_t Multiply_Mod(uint32_t x, uint16_t y, uint16_t modulo)
    {
        if  (x > y)
        {
            uint64_t tmp = x;
            x = y;
            y = tmp;
        }
        uint64_t res = 0;
        uint64_t iy  = y;
        while (x)
        {
            if (x & 1)//if odd
                res = (res  + iy) % modulo;
            iy  = (iy << 1) % modulo;
            x >>= 1;
        }
        return  res;
    }

    uint16_t Inverse(uint8_t value , uint16_t modulo)
    {
        uint16_t res = 1;
        uint16_t pow = 254, tmp = value;
        while (pow)
        {
            if (pow & 1)
                res = Multiply_Mod(res, tmp, modulo);
            tmp = Multiply_Mod(tmp, tmp, modulo);
            pow >>= 1;
        }
        return res;
    }

    uint16_t Divide_Mod(uint16_t a, uint8_t b)
    {
        auto myLambda = [](const uint16_t & value) -> uint16_t
        {
            uint16_t number = 0;
            uint16_t one = 1;

            for (uint16_t i = 0; i < 13; i++)
            {
                if (one & value)
                    number = i;
                one <<= 1;
            }
            return number;
        };

        uint16_t value_new;
        uint16_t degree_old = myLambda(b);
        uint16_t degree_new = myLambda(a);

        while (degree_old <= degree_new)
        {
            value_new = b << (degree_new - degree_old);
            a ^= value_new;
            degree_new = myLambda(a);
        }
        return a;
    }

    std::list<uint16_t> Irreducible_Poly()
    {
        std::list<uint16_t> list;
        bool flag = true;
        for (uint16_t i = 257; i < 512; i += 2)
        {
            for (uint8_t k = 3; k < 32; k++)
            {
                if (Divide_Mod(i, k) == 0)
                    flag = false;
            }
            if (flag)
                list.push_back(i);
            flag = true;
        }
        return list;
    }

};

void menu()
{
    std::cout << "\nWhat do you want?\n" << "1. Add\n" << "2. Multiplication\n" << "3. Inverse\n" << "4. List of irreducible polynomials\n" << "5. Exit\n" << "-> ";
}

int main()
{
    GaluaField field;
    int choice = 0;
    short int a, b, c;
    uint16_t module = 283;
    uint16_t tmp;

    while(choice != 5)
    {
        menu();
        std::cin >> choice;
        switch(choice)
        {
            case 1:
            {
                std::cout << "Enter two elements of field: ";
                std::cin >> a;
                std::cin >> b;
                c = field.Add(a, b);
                std::cout << "Result: " << static_cast<unsigned>(c) << std::endl;
                break;
            }

            case 2:
            {
                std::cout << "Enter two elements of field: ";
                std::cin >> a >> b;
                c = field.Multiply(a, b, module);
                std::cout << "Result: " << static_cast<unsigned>(c) << std::endl;
                break;
            }

            case 3:
            {
                std::cout << "Enter element of field: ";
                std::cin >> a;
                tmp = field.Inverse(a, module);
                std::cout << "Result: " << static_cast<unsigned>(tmp) << std::endl;
                break;
            }

            case 4:
            {
                std::list<uint16_t> list = field.Irreducible_Poly();
                for (auto iter = list.begin(); iter != list.end(); ++iter)
                    std::cout << *iter << ' ';
                std::cout << std::endl;
                break;
            }

            case 5:
                break;

            default:
                continue;
        }
    }

    return 0;
}