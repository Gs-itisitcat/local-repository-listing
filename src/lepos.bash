# Source this file in your .bashrc or .bash_profile
function lepos {
    if [[ "$@" == *"-l"* ]] || [[ "$@" == *"--list-only"* ]] || [[ "$@" == *"-h"* ]] || [[ "$@" == *"--help"* ]]; then
        lepol "$@"
    else
        local path
        path=$(lepol "$@")
        status=$?
        if [[ $status -ne 0 ]]; then
            echo "lepol failed with status $status"
            echo $path
            return $status
        fi

        if [[ $path != "" ]]; then
            echo "Change directory to $path"
            \cd "$path"
        else
            echo "No repository selected."
        fi
    fi
}
