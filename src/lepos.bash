# Source this file in your .bashrc or .bash_profile
function lepos {
    if [[ "$@" == *"-l"* ]] || [[ "$@" == *"--list-only"* ]] || [[ "$@" == *"-h"* ]] || [[ "$@" == *"--help"* ]]; then
        lepol "$@"
    else
        local path
        path=$(lepol "$@")
        if [[ $path != "" ]]; then
            echo "Change directory to $path"
            \cd "$path"
        else
            echo "No repository selected."
        fi
    fi
}
