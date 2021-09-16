#ifndef VEDAG_Utils_HEADERFILE
#define VEDAG_Utils_HEADERFILE
#pragma warning(disable:4503)
#include <string>
#include <vector>
namespace veda {
    //Thank you from https://tuttlem.github.io/2014/08/18/getting-istream-to-work-off-a-byte-array.html
    class membuf : public std::basic_streambuf<char> {
    public:
        membuf(const unsigned char *p, int l);
    };
}

#endif