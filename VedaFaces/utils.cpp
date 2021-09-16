
#include "utils.h"

namespace veda {
    //Thank you from https://tuttlem.github.io/2014/08/18/getting-istream-to-work-off-a-byte-array.html

    membuf::membuf(const unsigned char *p, int l) {
        setg((char*)p, (char*)p, (char*)p + l);
    }
}