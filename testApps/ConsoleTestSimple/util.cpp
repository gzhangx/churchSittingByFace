

#include <string>
#include <windows.h>
std::string getExeProgram() {
    char buf[512];
    GetModuleFileNameA(NULL, buf, 512);
    return std::string(buf);
}


std::string getExeDir() {
    auto filename = getExeProgram();
    const size_t last_slash_idx = filename.rfind('\\');
    if (std::string::npos != last_slash_idx)
    {
        return filename.substr(0, last_slash_idx);
    }
    return filename;
}