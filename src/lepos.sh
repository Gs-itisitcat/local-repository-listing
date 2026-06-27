# Source this file in your .bashrc, .bash_profile, or .zshrc
lepos() {
    local arg
    for arg in "$@"; do
        case "$arg" in
            -l|--list-only|-h|--help)
                lepol "$@" || return $?
                return 0
                ;;
        esac
    done

    local repo_path
    local exit_status=0
    repo_path=$(lepol "$@") || exit_status=$?
    if [[ $exit_status -eq 130 ]]; then
        echo "No repository selected."
        return 0
    elif [[ $exit_status -ne 0 ]]; then
        echo "lepol failed with status $exit_status"
        echo "$repo_path"
        return $exit_status
    fi

    if [[ -n "$repo_path" ]]; then
        echo "Change directory to $repo_path"
        \cd "$repo_path"
    else
        echo "No repository selected."
    fi
}
